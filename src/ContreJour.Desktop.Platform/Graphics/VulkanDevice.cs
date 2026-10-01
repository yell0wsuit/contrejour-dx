using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

using ContreJour.Desktop.Platform.Diagnostics;

using Microsoft.Extensions.Logging;

using SDL3;

using SkiaSharp;

namespace ContreJour.Desktop.Platform.Graphics
{
    // SDL Vulkan presentation with Skia drawing into a device-owned offscreen image (from
    // cuttherope-dx's VulkanDevice). Skia is never given a swapchain image. It renders into an image
    // this device owns, whose layout this device sets before handing it over, and the finished frame
    // reaches the swapchain through a GPU blit. Published SkiaSharp exposes neither a present-access
    // flush nor the image layout Skia left behind, so a swapchain image handed to Skia could not be
    // transitioned for presentation.
    public sealed unsafe class VulkanDevice(Action<string> fault) : SdlGraphicsDevice(fault)
    {
        private VulkanApi _vk;

        private nint _instance;

        private nint _physicalDevice;

        private nint _device;

        private nint _queue;

        private uint _queueFamily;

        private ulong _surface;

        private ulong _swapchain;

        private ulong[] _swapchainImages = [];

        private uint _swapchainFormat;

        private ulong _commandPool;

        private nint _commandBuffer;

        private ulong _acquireFence;

        // The offscreen image Skia draws into, blitted to the swapchain on present.
        private ulong _image;

        private ulong _imageMemory;

        // The layout this device last set on image.
        private uint _imageLayout;

        private string[] _instanceExtensions = [];

        private string[] _deviceExtensions = [];

        private uint _acquiredIndex;

        private bool _frameAcquired;

        public override GraphicsBackendKind Kind => GraphicsBackendKind.Vulkan;

        // Creates the SDL Vulkan window, instance, device and Skia context.
        public override void Initialize()
        {
            CheckThread();
            if (OperatingSystem.IsMacOS())
            {
                throw new PlatformNotSupportedException(
                    "SkiaSharp's macOS native library is built without Skia's Vulkan backend, so its " +
                    "Vulkan entry points return null however the device is configured. Installing " +
                    "MoltenVK does not change this. Use Metal on macOS.");
            }

            CreateWindow(SDL.WindowFlags.Vulkan);
            Check(SDL.VulkanLoadLibrary(null));
            Own(SDL.VulkanUnloadLibrary);
            nint procAddress = SDL.VulkanGetVkGetInstanceProcAddr();
            if (procAddress == 0)
            {
                throw new InvalidOperationException(SDL.GetError());
            }

            _vk = new VulkanApi(procAddress);
            _vk.LoadGlobal();
            CreateInstance();
            _vk.LoadInstance(_instance);
            SelectPhysicalDevice();
            CreateSurface();
            CreateDevice();
            Fault("after-device");
            CreateSkiaContext(procAddress);
            CreateCommandResources();
            // Registered first: a failure part-way through creating them must still release them.
            Own(DestroySwapchain);
            Own(DestroyImage);
            CreateSwapchain();
        }

        // The Khronos validation layer, in a development build on a machine that has it.
        // This is here for one assumption in particular. The offscreen image is handed to Skia
        // as a render target and taken back for the blit in BlitAndPresent, and the
        // layout that blit names as the image's current one is the layout this device last set.
        // Skia owns the image in between and nothing in SkiaSharp can be asked what it left:
        // there is no managed accessor, and libSkiaSharp exports none either, so the
        // barrier states a layout it cannot verify.
        // It holds in practice, because Skia leaves a render target in the layout it renders
        // into. What it is not is checked, and a barrier whose old layout is wrong is undefined
        // behaviour rather than an error - the frame would simply be wrong, on some drivers,
        // sometimes. The validation layer is what turns that into a message naming the mismatch,
        // so a future Skia that transitions on its way out is caught while someone is developing
        // rather than after it ships.
        // Release builds enable nothing, and a development machine without the Vulkan SDK
        // installed reports no layers and carries on unchecked.
        private static nint ValidationLayerName(VulkanApi api)
        {
#if DEBUG
            const string Validation = "VK_LAYER_KHRONOS_validation";
            uint count = 0;
            if (api.EnumerateInstanceLayerProperties(&count, null) != 0 || count == 0)
            {
                return 0;
            }

            VkLayerProperties[] layers = new VkLayerProperties[count];
            fixed (VkLayerProperties* reported = layers)
            {
                if (api.EnumerateInstanceLayerProperties(&count, reported) != 0)
                {
                    return 0;
                }

                for (uint index = 0; index < count; index++)
                {
                    if (Marshal.PtrToStringUTF8((nint)reported[index].LayerName) == Validation)
                    {
                        return Marshal.StringToCoTaskMemUTF8(Validation);
                    }
                }
            }
#endif
            return 0;
        }

        // Creates the instance with the surface extensions SDL requires.
        private void CreateInstance()
        {
            string[] required = SDL.VulkanGetInstanceExtensions(out _)
                ?? throw new InvalidOperationException(SDL.GetError());
            _instanceExtensions = required;
            nint applicationName = Marshal.StringToCoTaskMemUTF8("Contre Jour");
            nint[] extensionNames = new nint[required.Length];
            for (int index = 0; index < required.Length; index++)
            {
                extensionNames[index] = Marshal.StringToCoTaskMemUTF8(required[index]);
            }

            try
            {
                VkApplicationInfo application = new()
                {
                    Type = Vk.StructureApplicationInfo,
                    ApplicationName = (byte*)applicationName,
                    EngineName = (byte*)applicationName,
                    ApiVersion = Vk.ApiVersion11,
                };
                nint validationName = ValidationLayerName(_vk);
                nint[] layerNames = validationName == 0 ? [] : [validationName];
                fixed (nint* extensions = extensionNames)
                fixed (nint* layers = layerNames)
                {
                    VkInstanceCreateInfo create = new()
                    {
                        Type = Vk.StructureInstanceCreateInfo,
                        ApplicationInfo = &application,
                        EnabledLayerCount = (uint)layerNames.Length,
                        EnabledLayerNames = (byte**)layers,
                        EnabledExtensionCount = (uint)extensionNames.Length,
                        EnabledExtensionNames = (byte**)extensions,
                    };
                    nint created;
                    VulkanApi.Check(_vk.CreateInstance(&create, 0, &created), "vkCreateInstance");
                    _instance = created;
                }

                if (validationName != 0)
                {
                    Marshal.FreeCoTaskMem(validationName);
                }

                Own(() => _vk.DestroyInstance(_instance, 0));
            }
            finally
            {
                Marshal.FreeCoTaskMem(applicationName);
                foreach (nint name in extensionNames)
                {
                    Marshal.FreeCoTaskMem(name);
                }
            }
        }

        // Picks the first adapter with a graphics queue, preferring discrete hardware.
        private void SelectPhysicalDevice()
        {
            uint count = 0;
            VulkanApi.Check(_vk.EnumeratePhysicalDevices(_instance, &count, null), "vkEnumeratePhysicalDevices");
            if (count == 0)
            {
                throw new InvalidOperationException("No Vulkan adapter is available.");
            }

            nint[] candidates = new nint[count];
            fixed (nint* handles = candidates)
            {
                VulkanApi.Check(_vk.EnumeratePhysicalDevices(_instance, &count, handles), "vkEnumeratePhysicalDevices");
            }

            nint best = 0;
            uint bestType = uint.MaxValue;
            string bestVersion = "unknown";
            uint bestFamily = 0;
            string bestName = string.Empty;
            foreach (nint candidate in candidates)
            {
                if (!TryFindGraphicsQueue(candidate, out uint family))
                {
                    continue;
                }

                VkPhysicalDeviceProperties properties;
                _vk.GetPhysicalDeviceProperties(candidate, &properties);
                uint rank = properties.DeviceType switch
                {
                    2 => 0, // Discrete.
                    1 => 1, // Integrated.
                    3 => 2, // Virtual.
                    _ => 3, // CPU or other; accepted only when nothing else exists.
                };
                if (rank >= bestType)
                {
                    continue;
                }

                bestType = rank;
                best = candidate;
                bestFamily = family;
                bestName = Marshal.PtrToStringUTF8((nint)properties.DeviceName) ?? "unknown";

                // The API version is packed the way the specification defines, so it decodes for
                // every vendor. The driver version's packing is the vendor's own, so it is left
                // as the number they reported rather than decoded into something plausible but
                // wrong.
                bestVersion = string.Create(
                    CultureInfo.InvariantCulture,
                    $"API {properties.ApiVersion >> 22}.{(properties.ApiVersion >> 12) & 0x3FF}."
                        + $"{properties.ApiVersion & 0xFFF}, driver 0x{properties.DriverVersion:X8}");
            }

            if (best == 0)
            {
                throw new InvalidOperationException("No Vulkan adapter exposes a graphics queue.");
            }

            _physicalDevice = best;
            _queueFamily = bestFamily;
            string adapterType = bestType == 0 ? "discrete"
                : bestType == 1 ? "integrated"
                : bestType == 2 ? "virtual" : "software";
            ILogger logger = Log.For(LogCategories.Graphics);
            if (logger.IsEnabled(LogLevel.Information))
            {
                GraphicsDeviceLog.Adapter(logger, Kind, $"{bestName} ({adapterType})", bestVersion);
            }
        }

        // Finds a queue family that supports graphics work.
        private bool TryFindGraphicsQueue(nint candidate, out uint family)
        {
            uint count = 0;
            _vk.GetPhysicalDeviceQueueFamilyProperties(candidate, &count, null);
            VkQueueFamilyProperties[] families = new VkQueueFamilyProperties[count];
            fixed (VkQueueFamilyProperties* items = families)
            {
                _vk.GetPhysicalDeviceQueueFamilyProperties(candidate, &count, items);
            }

            for (uint index = 0; index < count; index++)
            {
                if ((families[index].QueueFlags & Vk.QueueGraphics) != 0)
                {
                    family = index;
                    return true;
                }
            }

            family = 0;
            return false;
        }

        // Creates the window surface through SDL and verifies the queue can present to it.
        private void CreateSurface()
        {
            if (!SDL.VulkanCreateSurface(Window, _instance, 0, out nint created) || created == 0)
            {
                throw new InvalidOperationException(SDL.GetError());
            }

            _surface = (ulong)created;
            Own(() => SDL.VulkanDestroySurface(_instance, (nint)_surface, 0));
            uint supported = 0;
            VulkanApi.Check(_vk.GetPhysicalDeviceSurfaceSupport(_physicalDevice, _queueFamily, _surface, &supported),
                "vkGetPhysicalDeviceSurfaceSupportKHR");
            if (supported == 0)
            {
                throw new InvalidOperationException("The graphics queue cannot present to this window surface.");
            }
        }

        // Creates the logical device with the swapchain and any required portability extension.
        private void CreateDevice()
        {
            List<string> required = ["VK_KHR_swapchain"];
            if (HasDeviceExtension("VK_KHR_portability_subset"))
            {
                required.Add("VK_KHR_portability_subset");
            }

            _deviceExtensions = [.. required];

            nint[] names = new nint[required.Count];
            for (int index = 0; index < required.Count; index++)
            {
                names[index] = Marshal.StringToCoTaskMemUTF8(required[index]);
            }

            try
            {
                float priority = 1.0f;
                VkDeviceQueueCreateInfo queueCreate = new()
                {
                    Type = Vk.StructureDeviceQueueCreateInfo,
                    QueueFamilyIndex = _queueFamily,
                    QueueCount = 1,
                    QueuePriorities = &priority,
                };
                fixed (nint* extensionNames = names)
                {
                    VkDeviceCreateInfo create = new()
                    {
                        Type = Vk.StructureDeviceCreateInfo,
                        QueueCreateInfoCount = 1,
                        QueueCreateInfos = &queueCreate,
                        EnabledExtensionCount = (uint)names.Length,
                        EnabledExtensionNames = (byte**)extensionNames,
                    };
                    nint created;
                    VulkanApi.Check(_vk.CreateDevice(_physicalDevice, &create, 0, &created), "vkCreateDevice");
                    _device = created;
                }

                Own(() => _vk.DestroyDevice(_device, 0));
            }
            finally
            {
                foreach (nint name in names)
                {
                    Marshal.FreeCoTaskMem(name);
                }
            }

            _vk.LoadDevice(_instance);
            nint acquired;
            _vk.GetDeviceQueue(_device, _queueFamily, 0, &acquired);
            _queue = acquired;
        }

        // Reports whether the selected adapter offers an extension.
        private bool HasDeviceExtension(string name)
        {
            uint count = 0;
            VulkanApi.Check(_vk.EnumerateDeviceExtensionProperties(_physicalDevice, null, &count, null),
                "vkEnumerateDeviceExtensionProperties");
            VkExtensionProperties[] available = new VkExtensionProperties[count];
            fixed (VkExtensionProperties* items = available)
            {
                VulkanApi.Check(_vk.EnumerateDeviceExtensionProperties(_physicalDevice, null, &count, items),
                    "vkEnumerateDeviceExtensionProperties");
                for (uint index = 0; index < count; index++)
                {
                    if (Marshal.PtrToStringUTF8((nint)items[index].ExtensionName) == name)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Builds the Skia Vulkan context over this device's instance, adapter and queue.
        private void CreateSkiaContext(nint procAddress)
        {
            delegate* unmanaged<nint, byte*, nint> resolveInstance = (delegate* unmanaged<nint, byte*, nint>)procAddress;
            delegate* unmanaged<nint, byte*, nint> resolveDevice = _vk.GetDeviceProcAddress;
            nint GetProcedure(string name, nint forInstance, nint forDevice)
            {
                nint utf8 = Marshal.StringToCoTaskMemUTF8(name);
                try
                {
                    // Device-level functions resolve against the device first, as Vulkan intends; the
                    // instance lookup is the documented fallback and also serves global functions.
                    nint address = forDevice != 0 ? resolveDevice(forDevice, (byte*)utf8) : 0;
                    return address != 0 ? address : resolveInstance(forInstance, (byte*)utf8);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(utf8);
                }
            }
            GRVkGetProcedureAddressDelegate procedure = GetProcedure;
            GRVkExtensions extensions = Own(GRVkExtensions.Create(procedure, _instance, _physicalDevice,
                _instanceExtensions, _deviceExtensions));
            GRVkBackendContext backend = Own(new GRVkBackendContext
            {
                Extensions = extensions,
                VkInstance = _instance,
                VkPhysicalDevice = _physicalDevice,
                VkDevice = _device,
                VkQueue = _queue,
                GraphicsQueueIndex = _queueFamily,
                MaxAPIVersion = Vk.ApiVersion11,
                GetProcedureAddress = procedure,
            });
            Context = Own(GRContext.CreateVulkan(backend)
                ?? throw new InvalidOperationException(
                    "Skia rejected this Vulkan device. Skia reports no detail; the usual causes are an " +
                    "adapter below Skia's feature requirements or a native library built without the " +
                    "Vulkan backend."));
        }

        // Creates the command pool, blit command buffer and acquisition fence.
        private void CreateCommandResources()
        {
            VkCommandPoolCreateInfo poolCreate = new()
            {
                Type = Vk.StructureCommandPoolCreateInfo,
                Flags = Vk.CommandPoolResetBuffer,
                QueueFamilyIndex = _queueFamily,
            };
            ulong createdPool;
            VulkanApi.Check(_vk.CreateCommandPool(_device, &poolCreate, 0, &createdPool), "vkCreateCommandPool");
            _commandPool = createdPool;
            Own(() => _vk.DestroyCommandPool(_device, _commandPool, 0));

            VkCommandBufferAllocateInfo bufferAllocate = new()
            {
                Type = Vk.StructureCommandBufferAllocateInfo,
                CommandPool = _commandPool,
                Level = Vk.CommandBufferLevelPrimary,
                CommandBufferCount = 1,
            };
            nint allocated;
            VulkanApi.Check(_vk.AllocateCommandBuffers(_device, &bufferAllocate, &allocated), "vkAllocateCommandBuffers");
            _commandBuffer = allocated;

            VkFenceCreateInfo fenceCreate = new() { Type = Vk.StructureFenceCreateInfo };
            ulong createdFence;
            VulkanApi.Check(_vk.CreateFence(_device, &fenceCreate, 0, &createdFence), "vkCreateFence");
            _acquireFence = createdFence;
            Own(() => _vk.DestroyFence(_device, _acquireFence, 0));
        }

        // Creates the swapchain for the window's current drawable size.
        private void CreateSwapchain()
        {
            VkSurfaceCapabilities capabilities;
            VulkanApi.Check(_vk.GetPhysicalDeviceSurfaceCapabilities(_physicalDevice, _surface, &capabilities),
                "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");

            uint formatCount = 0;
            VulkanApi.Check(_vk.GetPhysicalDeviceSurfaceFormats(_physicalDevice, _surface, &formatCount, null),
                "vkGetPhysicalDeviceSurfaceFormatsKHR");
            VkSurfaceFormat[] formats = new VkSurfaceFormat[formatCount];
            fixed (VkSurfaceFormat* items = formats)
            {
                VulkanApi.Check(_vk.GetPhysicalDeviceSurfaceFormats(_physicalDevice, _surface, &formatCount, items),
                    "vkGetPhysicalDeviceSurfaceFormatsKHR");
            }

            _swapchainFormat = Vk.FormatB8G8R8A8Unorm;
            bool supported = false;
            foreach (VkSurfaceFormat format in formats)
            {
                if (format.Format == _swapchainFormat && format.ColorSpace == Vk.ColorSpaceSrgbNonlinear)
                {
                    supported = true;
                    break;
                }
            }

            if (!supported)
            {
                throw new InvalidOperationException("The surface does not support a BGRA8 non-linear sRGB format.");
            }

            VkExtent2D extent = capabilities.CurrentExtent;
            if (extent.Width == uint.MaxValue)
            {
                Check(SDL.GetWindowSizeInPixels(Window, out int windowWidth, out int windowHeight));
                extent.Width = Math.Clamp((uint)windowWidth, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width);
                extent.Height = Math.Clamp((uint)windowHeight, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height);
            }

            uint imageCount = capabilities.MinImageCount + 1;
            if (capabilities.MaxImageCount != 0 && imageCount > capabilities.MaxImageCount)
            {
                imageCount = capabilities.MaxImageCount;
            }

            VkSwapchainCreateInfo create = new()
            {
                Type = Vk.StructureSwapchainCreateInfoKhr,
                Surface = _surface,
                MinImageCount = imageCount,
                ImageFormat = _swapchainFormat,
                ImageColorSpace = Vk.ColorSpaceSrgbNonlinear,
                ImageExtent = extent,
                ImageArrayLayers = 1,
                ImageUsage = Vk.UsageTransferDestination | Vk.UsageColorAttachment,
                ImageSharingMode = Vk.SharingExclusive,
                PreTransform = (capabilities.SupportedTransforms & Vk.SurfaceTransformIdentity) != 0
                    ? Vk.SurfaceTransformIdentity
                    : capabilities.CurrentTransform,
                CompositeAlpha = Vk.CompositeAlphaOpaque,
                PresentMode = Vk.PresentModeFifo,
                Clipped = 1,
            };
            ulong created;
            VulkanApi.Check(_vk.CreateSwapchain(_device, &create, 0, &created), "vkCreateSwapchainKHR");
            _swapchain = created;

            uint count = 0;
            VulkanApi.Check(_vk.GetSwapchainImages(_device, _swapchain, &count, null), "vkGetSwapchainImagesKHR");
            _swapchainImages = new ulong[count];
            fixed (ulong* items = _swapchainImages)
            {
                VulkanApi.Check(_vk.GetSwapchainImages(_device, _swapchain, &count, items), "vkGetSwapchainImagesKHR");
            }

            Width = (int)extent.Width;
            Height = (int)extent.Height;
            CreateOffscreenImage();
        }

        // Creates the image Skia renders into for the current swapchain size.
        private void CreateOffscreenImage()
        {
            VkImageCreateInfo create = new()
            {
                Type = Vk.StructureImageCreateInfo,
                ImageType = Vk.ImageType2D,
                Format = _swapchainFormat,
                Extent = new VkExtent3D { Width = (uint)Width, Height = (uint)Height, Depth = 1 },
                MipLevels = 1,
                ArrayLayers = 1,
                Samples = Vk.SampleCount1,
                Tiling = Vk.TilingOptimal,
                Usage = Vk.UsageColorAttachment | Vk.UsageTransferSource | Vk.UsageTransferDestination | Vk.UsageSampled,
                SharingMode = Vk.SharingExclusive,
                InitialLayout = Vk.LayoutUndefined,
            };
            ulong created;
            VulkanApi.Check(_vk.CreateImage(_device, &create, 0, &created), "vkCreateImage");
            _image = created;

            VkMemoryRequirements requirements;
            _vk.GetImageMemoryRequirements(_device, _image, &requirements);
            VkMemoryAllocateInfo allocate = new()
            {
                Type = Vk.StructureMemoryAllocateInfo,
                AllocationSize = requirements.Size,
                MemoryTypeIndex = FindMemoryType(requirements.MemoryTypeBits, Vk.MemoryDeviceLocal),
            };
            ulong allocated;
            VulkanApi.Check(_vk.AllocateMemory(_device, &allocate, 0, &allocated), "vkAllocateMemory");
            _imageMemory = allocated;
            VulkanApi.Check(_vk.BindImageMemory(_device, _image, _imageMemory, 0), "vkBindImageMemory");
            _imageLayout = Vk.LayoutUndefined;
        }

        // Finds a memory type satisfying both the image's bits and the required properties.
        private uint FindMemoryType(uint typeBits, uint properties)
        {
            VkPhysicalDeviceMemoryProperties memory;
            _vk.GetPhysicalDeviceMemoryProperties(_physicalDevice, &memory);
            for (uint index = 0; index < memory.MemoryTypeCount; index++)
            {
                if ((typeBits & (1u << (int)index)) != 0 && (memory.MemoryTypes[(int)index].PropertyFlags & properties) == properties)
                {
                    return index;
                }
            }

            throw new InvalidOperationException("No Vulkan memory type satisfies the offscreen image.");
        }

        public override bool AcquireFrame()
        {
            if (!GetDrawableSize(out int width, out int height))
            {
                return false;
            }

            // A resize with no drawable to build against leaves the swapchain destroyed and the
            // last size behind, so a window restored to the size it had would pass a null handle
            // to the driver if the size comparison were the only thing asked.
            if (_swapchain == 0 || width != Width || height != Height)
            {
                Resize();
                if (_swapchain == 0 || !GetDrawableSize(out _, out _))
                {
                    return false;
                }
            }

            uint index = 0;
            fixed (ulong* fence = &_acquireFence)
            {
                int acquired = _vk.AcquireNextImage(_device, _swapchain, Vk.WholeTimeout, 0, _acquireFence, &index);
                if (acquired is Vk.ErrorOutOfDateKhr)
                {
                    Resize();
                    return false;
                }

                if (acquired is not (Vk.Success or Vk.SuboptimalKhr))
                {
                    VulkanApi.Check(acquired, "vkAcquireNextImageKHR");
                }

                VulkanApi.Check(_vk.WaitForFences(_device, 1, fence, 1, Vk.WholeTimeout), "vkWaitForFences");
                VulkanApi.Check(_vk.ResetFences(_device, 1, fence), "vkResetFences");
            }

            _acquiredIndex = index;
            _frameAcquired = true;
            Fault("before-surface");
            TransitionOffscreenImage(Vk.LayoutColorAttachmentOptimal);
            SetSurface(
                new GRBackendRenderTarget(Width, Height, new GRVkImageInfo
                {
                    Image = _image,
                    ImageLayout = _imageLayout,
                    ImageTiling = Vk.TilingOptimal,
                    ImageUsageFlags = Vk.UsageColorAttachment | Vk.UsageTransferSource | Vk.UsageTransferDestination | Vk.UsageSampled,
                    Format = _swapchainFormat,
                    LevelCount = 1,
                    SampleCount = 1,
                    SharingMode = Vk.SharingExclusive,
                    CurrentQueueFamily = _queueFamily,
                }),
                GRSurfaceOrigin.TopLeft,
                SKColorType.Bgra8888);
            Fault("after-surface");
            return true;
        }

        public override void Present()
        {
            CheckThread();
            if (!_frameAcquired)
            {
                throw new InvalidOperationException("No acquired Vulkan swapchain image to present.");
            }

            ClearSurface();
            BlitAndPresent();
            _frameAcquired = false;
        }

        // Blits the finished offscreen image onto the acquired swapchain image and presents it.
        private void BlitAndPresent()
        {
            VulkanApi.Check(_vk.ResetCommandPool(_device, _commandPool, 0), "vkResetCommandPool");
            VkCommandBufferBeginInfo begin = new()
            {
                Type = Vk.StructureCommandBufferBeginInfo,
                Flags = Vk.CommandBufferOneTimeSubmit,
            };
            VulkanApi.Check(_vk.BeginCommandBuffer(_commandBuffer, &begin), "vkBeginCommandBuffer");

            ulong target = _swapchainImages[_acquiredIndex];
            RecordBarrier(_image, _imageLayout, Vk.LayoutTransferSourceOptimal,
                Vk.AccessColorAttachmentWrite, Vk.AccessTransferRead, Vk.StageAllCommands, Vk.StageTransfer);
            _imageLayout = Vk.LayoutTransferSourceOptimal;
            RecordBarrier(target, Vk.LayoutUndefined, Vk.LayoutTransferDestinationOptimal,
                Vk.AccessNone, Vk.AccessTransferWrite, Vk.StageTopOfPipe, Vk.StageTransfer);

            VkImageBlit blit = new()
            {
                SourceSubresource = new VkImageSubresourceLayers { AspectMask = Vk.AspectColor, LayerCount = 1 },
                SourceOffset1 = new VkOffset3D { X = Width, Y = Height, Z = 1 },
                DestinationSubresource = new VkImageSubresourceLayers { AspectMask = Vk.AspectColor, LayerCount = 1 },
                DestinationOffset1 = new VkOffset3D { X = Width, Y = Height, Z = 1 },
            };
            _vk.CmdBlitImage(_commandBuffer, _image, Vk.LayoutTransferSourceOptimal, target,
                Vk.LayoutTransferDestinationOptimal, 1, &blit, 0);

            RecordBarrier(target, Vk.LayoutTransferDestinationOptimal, Vk.LayoutPresentSourceKhr,
                Vk.AccessTransferWrite, Vk.AccessMemoryRead, Vk.StageTransfer, Vk.StageBottomOfPipe);
            VulkanApi.Check(_vk.EndCommandBuffer(_commandBuffer), "vkEndCommandBuffer");

            fixed (nint* buffers = &_commandBuffer)
            {
                VkSubmitInfo submit = new()
                {
                    Type = Vk.StructureSubmitInfo,
                    CommandBufferCount = 1,
                    CommandBuffers = buffers,
                };
                VulkanApi.Check(_vk.QueueSubmit(_queue, 1, &submit, 0), "vkQueueSubmit");
            }

            // The prototype keeps one frame in flight, so the queue drains before presenting.
            VulkanApi.Check(_vk.QueueWaitIdle(_queue), "vkQueueWaitIdle");

            fixed (ulong* swapchains = &_swapchain)
            fixed (uint* indices = &_acquiredIndex)
            {
                VkPresentInfo present = new()
                {
                    Type = Vk.StructurePresentInfoKhr,
                    SwapchainCount = 1,
                    Swapchains = swapchains,
                    ImageIndices = indices,
                };
                int presented = _vk.QueuePresent(_queue, &present);
                if (presented is Vk.ErrorOutOfDateKhr or Vk.SuboptimalKhr)
                {
                    Resize();
                    return;
                }

                VulkanApi.Check(presented, "vkQueuePresentKHR");
            }
        }

        // Transitions the offscreen image on its own submission, outside frame recording.
        private void TransitionOffscreenImage(uint layout)
        {
            if (_imageLayout == layout)
            {
                return;
            }

            VulkanApi.Check(_vk.ResetCommandPool(_device, _commandPool, 0), "vkResetCommandPool");
            VkCommandBufferBeginInfo begin = new()
            {
                Type = Vk.StructureCommandBufferBeginInfo,
                Flags = Vk.CommandBufferOneTimeSubmit,
            };
            VulkanApi.Check(_vk.BeginCommandBuffer(_commandBuffer, &begin), "vkBeginCommandBuffer");
            RecordBarrier(_image, _imageLayout, layout, Vk.AccessNone, Vk.AccessColorAttachmentWrite,
                Vk.StageTopOfPipe, Vk.StageColorAttachmentOutput);
            VulkanApi.Check(_vk.EndCommandBuffer(_commandBuffer), "vkEndCommandBuffer");
            fixed (nint* buffers = &_commandBuffer)
            {
                VkSubmitInfo submit = new()
                {
                    Type = Vk.StructureSubmitInfo,
                    CommandBufferCount = 1,
                    CommandBuffers = buffers,
                };
                VulkanApi.Check(_vk.QueueSubmit(_queue, 1, &submit, 0), "vkQueueSubmit");
            }

            VulkanApi.Check(_vk.QueueWaitIdle(_queue), "vkQueueWaitIdle");
            _imageLayout = layout;
        }

        // Records one image layout transition into the open command buffer.
        private void RecordBarrier(ulong subject, uint oldLayout, uint newLayout, uint sourceAccess,
            uint destinationAccess, uint sourceStage, uint destinationStage)
        {
            VkImageMemoryBarrier barrier = new()
            {
                Type = Vk.StructureImageMemoryBarrier,
                SourceAccessMask = sourceAccess,
                DestinationAccessMask = destinationAccess,
                OldLayout = oldLayout,
                NewLayout = newLayout,
                SourceQueueFamilyIndex = uint.MaxValue,
                DestinationQueueFamilyIndex = uint.MaxValue,
                Image = subject,
                SubresourceRange = new VkImageSubresourceRange
                {
                    AspectMask = Vk.AspectColor,
                    LevelCount = 1,
                    LayerCount = 1,
                },
            };
            _vk.CmdPipelineBarrier(_commandBuffer, sourceStage, destinationStage, 0, 0, null, 0, null, 1, &barrier);
        }

        // Rebuilds the swapchain and offscreen image for the window's current size, or leaves both
        // released while the window has no area.
        private void Resize()
        {
            CheckThread();
            Context.Flush(submit: true, synchronous: true);
            ClearSurface();
            _frameAcquired = false;
            VulkanApi.Check(_vk.DeviceWaitIdle(_device), "vkDeviceWaitIdle");
            Context.ResetContext();
            DestroySwapchain();
            DestroyImage();
            if (GetDrawableSize(out _, out _))
            {
                CreateSwapchain();
            }
        }

        // Releases the swapchain and its image handles.
        private void DestroySwapchain()
        {
            if (_swapchain == 0)
            {
                return;
            }

            _vk.DestroySwapchain(_device, _swapchain, 0);
            _swapchain = 0;
            _swapchainImages = [];
        }

        // Releases the offscreen image and its memory.
        private void DestroyImage()
        {
            if (_image != 0)
            {
                _vk.DestroyImage(_device, _image, 0);
                _image = 0;
            }

            if (_imageMemory != 0)
            {
                _vk.FreeMemory(_device, _imageMemory, 0);
                _imageMemory = 0;
            }

            _imageLayout = Vk.LayoutUndefined;
        }
    }
}
