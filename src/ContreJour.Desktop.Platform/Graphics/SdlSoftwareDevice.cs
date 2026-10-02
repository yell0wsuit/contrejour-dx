using System;


using Microsoft.Extensions.Logging;

using Mokus2D.Diagnostics;

using SDL3;

using SkiaSharp;

namespace ContreJour.Desktop.Platform.Graphics
{
    // Skia drawing on the CPU into a bitmap, presented through SDL's software renderer: the fallback
    // for machines without a usable GPU driver.
    public sealed class SdlSoftwareDevice(Action<string> fault) : SdlGraphicsDevice(fault)
    {
        private nint _renderer;

        private nint _texture;

        private SKBitmap _pixels;

        public override GraphicsBackendKind Kind => GraphicsBackendKind.Software;

        public override void Initialize()
        {
            CheckThread();
            // Held for the device's lifetime and released after its window: SDL reads it when the
            // window's framebuffer is made, and it must not outlive this device.
            GlHintScope hints = Own(new GlHintScope());
            // Cocoa has no native window framebuffer; SDL must upload the CPU raster for
            // presentation there. Other platforms keep the GPU-independent framebuffer path.
            hints.Set(SDL.Hints.FramebufferAcceleration, OperatingSystem.IsMacOS() ? "1" : "0");
            CreateWindow(0);
            _renderer = SDL.CreateRenderer(Window, "software");
            if (_renderer == 0)
            {
                throw new InvalidOperationException($"SDL could not create a software renderer: {SDL.GetError()}");
            }
            nint renderer = _renderer;
            Own(() => SDL.DestroyRenderer(renderer));
            Own(ReleasePixels);
            ILogger logger = Log.For(LogCategories.Graphics);
            GraphicsDeviceLog.Adapter(logger, Kind, "Skia raster through the SDL software renderer", "CPU");
            Fault("after-device");
        }

        public override bool AcquireFrame()
        {
            if (!GetDrawableSize(out int width, out int height))
            {
                return false;
            }
            if (!HasFrame || width != Width || height != Height)
            {
                Resize(width, height);
            }
            return true;
        }

        public override void Present()
        {
            CheckThread();
            Check(SDL.UpdateTexture(_texture, 0, _pixels.GetPixels(), _pixels.RowBytes));
            Check(SDL.RenderTexture(_renderer, _texture, 0, 0));
            Check(SDL.RenderPresent(_renderer));
        }

        private void Resize(int width, int height)
        {
            ClearSurface();
            ReleasePixels();
            Fault("before-surface");
            _pixels = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul));
            SetRasterSurface(SKSurface.Create(_pixels.Info, _pixels.GetPixels(), _pixels.RowBytes));
            // Desktop targets are little-endian, where ABGR8888 stores its bytes in RGBA order.
            _texture = SDL.CreateTexture(_renderer, SDL.PixelFormat.ABGR8888, SDL.TextureAccess.Streaming, width, height);
            if (_texture == 0)
            {
                throw new InvalidOperationException($"SDL could not create the frame texture: {SDL.GetError()}");
            }
            Check(SDL.SetTextureBlendMode(_texture, SDL.BlendMode.None));
            Width = width;
            Height = height;
            Fault("after-surface");
        }

        private void ReleasePixels()
        {
            if (_texture != 0)
            {
                SDL.DestroyTexture(_texture);
                _texture = 0;
            }
            _pixels?.Dispose();
            _pixels = null;
        }
    }
}
