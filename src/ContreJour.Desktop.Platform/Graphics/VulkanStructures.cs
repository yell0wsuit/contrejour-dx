using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ContreJour.Desktop.Platform.Graphics
{
    // A two-dimensional size, matching VkExtent2D.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkExtent2D
    {
        internal uint Width;
        internal uint Height;
    }

    // A three-dimensional size, matching VkExtent3D.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkExtent3D
    {
        internal uint Width;
        internal uint Height;
        internal uint Depth;
    }

    // A three-dimensional signed offset, matching VkOffset3D.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkOffset3D
    {
        internal int X;
        internal int Y;
        internal int Z;
    }

    // Application and API version identification, matching VkApplicationInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkApplicationInfo
    {
        internal uint Type;
        internal void* Next;
        internal byte* ApplicationName;
        internal uint ApplicationVersion;
        internal byte* EngineName;
        internal uint EngineVersion;
        internal uint ApiVersion;
    }

    // Instance creation parameters, matching VkInstanceCreateInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkInstanceCreateInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint Flags;
        internal VkApplicationInfo* ApplicationInfo;
        internal uint EnabledLayerCount;
        internal byte** EnabledLayerNames;
        internal uint EnabledExtensionCount;
        internal byte** EnabledExtensionNames;
    }

    // Queue creation parameters, matching VkDeviceQueueCreateInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkDeviceQueueCreateInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint Flags;
        internal uint QueueFamilyIndex;
        internal uint QueueCount;
        internal float* QueuePriorities;
    }

    // Logical device creation parameters, matching VkDeviceCreateInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkDeviceCreateInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint Flags;
        internal uint QueueCreateInfoCount;
        internal VkDeviceQueueCreateInfo* QueueCreateInfos;
        internal uint EnabledLayerCount;
        internal byte** EnabledLayerNames;
        internal uint EnabledExtensionCount;
        internal byte** EnabledExtensionNames;
        internal void* EnabledFeatures;
    }

    // Adapter identification, matching the leading fields of VkPhysicalDeviceProperties.
    // Only the fields up to the device name are read. The trailing reservation covers the limits and
    // sparse properties this backend never inspects, and is deliberately larger than the native
    // structure so the driver can never write past it.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkPhysicalDeviceProperties
    {
        internal uint ApiVersion;
        internal uint DriverVersion;
        internal uint VendorId;
        internal uint DeviceId;
        internal uint DeviceType;
        internal fixed byte DeviceName[256];
        internal fixed byte PipelineCacheUuid[16];
        internal fixed byte Reserved[1024];
    }

    // Queue family capabilities, matching VkQueueFamilyProperties.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkQueueFamilyProperties
    {
        internal uint QueueFlags;
        internal uint QueueCount;
        internal uint TimestampValidBits;
        internal VkExtent3D MinImageTransferGranularity;
    }

    // One reported extension, matching VkExtensionProperties.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkExtensionProperties
    {
        internal fixed byte ExtensionName[256];
        internal uint SpecVersion;
    }

    // One reported instance layer, matching VkLayerProperties.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkLayerProperties
    {
        internal fixed byte LayerName[256];
        internal uint SpecVersion;
        internal uint ImplementationVersion;
        internal fixed byte Description[256];
    }

    // One memory type, matching VkMemoryType.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkMemoryType
    {
        internal uint PropertyFlags;
        internal uint HeapIndex;
    }

    // One memory heap, matching VkMemoryHeap.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkMemoryHeap
    {
        internal ulong Size;
        internal uint Flags;
        private readonly uint padding;
    }

    // Device memory layout, matching VkPhysicalDeviceMemoryProperties.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkPhysicalDeviceMemoryProperties
    {
        internal uint MemoryTypeCount;
        internal VkMemoryTypeArray MemoryTypes;
        internal uint MemoryHeapCount;
        internal VkMemoryHeapArray MemoryHeaps;
    }

    // The fixed 32-entry memory type table Vulkan reports.
    [StructLayout(LayoutKind.Sequential)]
    [InlineArray(32)]
    internal struct VkMemoryTypeArray
    {
        private VkMemoryType element;
    }

    // The fixed 16-entry memory heap table Vulkan reports.
    [StructLayout(LayoutKind.Sequential)]
    [InlineArray(16)]
    internal struct VkMemoryHeapArray
    {
        private VkMemoryHeap element;
    }

    // Surface limits, matching VkSurfaceCapabilitiesKHR.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkSurfaceCapabilities
    {
        internal uint MinImageCount;
        internal uint MaxImageCount;
        internal VkExtent2D CurrentExtent;
        internal VkExtent2D MinImageExtent;
        internal VkExtent2D MaxImageExtent;
        internal uint MaxImageArrayLayers;
        internal uint SupportedTransforms;
        internal uint CurrentTransform;
        internal uint SupportedCompositeAlpha;
        internal uint SupportedUsageFlags;
    }

    // One supported surface format, matching VkSurfaceFormatKHR.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkSurfaceFormat
    {
        internal uint Format;
        internal uint ColorSpace;
    }

    // Swapchain creation parameters, matching VkSwapchainCreateInfoKHR.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkSwapchainCreateInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint Flags;
        internal ulong Surface;
        internal uint MinImageCount;
        internal uint ImageFormat;
        internal uint ImageColorSpace;
        internal VkExtent2D ImageExtent;
        internal uint ImageArrayLayers;
        internal uint ImageUsage;
        internal uint ImageSharingMode;
        internal uint QueueFamilyIndexCount;
        internal uint* QueueFamilyIndices;
        internal uint PreTransform;
        internal uint CompositeAlpha;
        internal uint PresentMode;
        internal uint Clipped;
        internal ulong OldSwapchain;
    }

    // Presentation parameters, matching VkPresentInfoKHR.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkPresentInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint WaitSemaphoreCount;
        internal ulong* WaitSemaphores;
        internal uint SwapchainCount;
        internal ulong* Swapchains;
        internal uint* ImageIndices;
        internal int* Results;
    }

    // Image creation parameters, matching VkImageCreateInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkImageCreateInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint Flags;
        internal uint ImageType;
        internal uint Format;
        internal VkExtent3D Extent;
        internal uint MipLevels;
        internal uint ArrayLayers;
        internal uint Samples;
        internal uint Tiling;
        internal uint Usage;
        internal uint SharingMode;
        internal uint QueueFamilyIndexCount;
        internal uint* QueueFamilyIndices;
        internal uint InitialLayout;
    }

    // Allocation requirements, matching VkMemoryRequirements.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkMemoryRequirements
    {
        internal ulong Size;
        internal ulong Alignment;
        internal uint MemoryTypeBits;
    }

    // Allocation parameters, matching VkMemoryAllocateInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkMemoryAllocateInfo
    {
        internal uint Type;
        internal void* Next;
        internal ulong AllocationSize;
        internal uint MemoryTypeIndex;
    }

    // Command pool creation parameters, matching VkCommandPoolCreateInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkCommandPoolCreateInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint Flags;
        internal uint QueueFamilyIndex;
    }

    // Command buffer allocation parameters, matching VkCommandBufferAllocateInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkCommandBufferAllocateInfo
    {
        internal uint Type;
        internal void* Next;
        internal ulong CommandPool;
        internal uint Level;
        internal uint CommandBufferCount;
    }

    // Command buffer recording parameters, matching VkCommandBufferBeginInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkCommandBufferBeginInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint Flags;
        internal void* InheritanceInfo;
    }

    // Queue submission parameters, matching VkSubmitInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkSubmitInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint WaitSemaphoreCount;
        internal ulong* WaitSemaphores;
        internal uint* WaitDstStageMask;
        internal uint CommandBufferCount;
        internal nint* CommandBuffers;
        internal uint SignalSemaphoreCount;
        internal ulong* SignalSemaphores;
    }

    // Fence creation parameters, matching VkFenceCreateInfo.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkFenceCreateInfo
    {
        internal uint Type;
        internal void* Next;
        internal uint Flags;
    }

    // A global memory barrier, matching VkMemoryBarrier.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkMemoryBarrier
    {
        internal uint Type;
        internal void* Next;
        internal uint SourceAccessMask;
        internal uint DestinationAccessMask;
    }

    // A buffer memory barrier, matching VkBufferMemoryBarrier.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkBufferMemoryBarrier
    {
        internal uint Type;
        internal void* Next;
        internal uint SourceAccessMask;
        internal uint DestinationAccessMask;
        internal uint SourceQueueFamilyIndex;
        internal uint DestinationQueueFamilyIndex;
        internal ulong Buffer;
        internal ulong Offset;
        internal ulong Size;
    }

    // A subresource span, matching VkImageSubresourceRange.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkImageSubresourceRange
    {
        internal uint AspectMask;
        internal uint BaseMipLevel;
        internal uint LevelCount;
        internal uint BaseArrayLayer;
        internal uint LayerCount;
    }

    // A subresource selection, matching VkImageSubresourceLayers.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkImageSubresourceLayers
    {
        internal uint AspectMask;
        internal uint MipLevel;
        internal uint BaseArrayLayer;
        internal uint LayerCount;
    }

    // An image layout transition, matching VkImageMemoryBarrier.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VkImageMemoryBarrier
    {
        internal uint Type;
        internal void* Next;
        internal uint SourceAccessMask;
        internal uint DestinationAccessMask;
        internal uint OldLayout;
        internal uint NewLayout;
        internal uint SourceQueueFamilyIndex;
        internal uint DestinationQueueFamilyIndex;
        internal ulong Image;
        internal VkImageSubresourceRange SubresourceRange;
    }

    // A blit region, matching VkImageBlit.
    [StructLayout(LayoutKind.Sequential)]
    internal struct VkImageBlit
    {
        internal VkImageSubresourceLayers SourceSubresource;
        internal VkOffset3D SourceOffset0;
        internal VkOffset3D SourceOffset1;
        internal VkImageSubresourceLayers DestinationSubresource;
        internal VkOffset3D DestinationOffset0;
        internal VkOffset3D DestinationOffset1;
    }
}
