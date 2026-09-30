using System;

using SDL3;

using SkiaSharp;

namespace ContreJour.Desktop.Platform.Graphics
{
    // An SDL window with an OpenGL context and a Skia surface over its default framebuffer (trimmed
    // from cuttherope-dx's SdlGraphicsDevice/SdlGlDevice). Everything here runs on the main thread.
    public sealed class SdlGlDevice : IDisposable
    {
        private const uint GlFramebufferBinding = 0x8CA6;

        private const uint GlRgba8 = 0x8058;

        private nint _glContext;

        private uint _framebuffer;

        private GRGlInterface _glInterface;

        private GRContext _grContext;

        private GRBackendRenderTarget _target;

        private SKSurface _surface;

        private bool _disposed;

        private SdlGlDevice()
        {
        }

        public nint Window { get; private set; }

        // Pixel size of the current surface.
        public int Width { get; private set; }

        public int Height { get; private set; }

        public SKCanvas Canvas => _surface?.Canvas ?? throw new InvalidOperationException("No frame is acquired.");

        // Creates a hidden window with a GL context; the caller sizes, places and shows it.
        public static SdlGlDevice Create(string title)
        {
            SdlGlDevice device = new();
            try
            {
                device.Initialize(title);
                return device;
            }
            catch
            {
                device.Dispose();
                throw;
            }
        }

        // False while the window is minimized or has no area, when there is nothing to draw into.
        public bool AcquireFrame()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if ((SDL.GetWindowFlags(Window) & SDL.WindowFlags.Minimized) != 0)
            {
                return false;
            }
            Check(SDL.GetWindowSizeInPixels(Window, out int width, out int height));
            if (width <= 0 || height <= 0)
            {
                return false;
            }
            Check(SDL.GLMakeCurrent(Window, _glContext));
            if (_surface == null || width != Width || height != Height)
            {
                CreateSurface(width, height);
            }
            return true;
        }

        public void Flush()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _grContext.Flush(submit: true, synchronous: true);
            // Skia abandons a context when the driver reports the device gone; every later draw
            // would be dropped silently.
            if (_grContext.IsAbandoned)
            {
                throw new InvalidOperationException("Skia abandoned the GL context.");
            }
        }

        public void Present()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!SDL.GLSwapWindow(Window))
            {
                throw new InvalidOperationException($"SDL could not swap the GL window: {SDL.GetError()}");
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            ReleaseSurface();
            _grContext?.Dispose();
            _grContext = null;
            _glInterface?.Dispose();
            _glInterface = null;
            if (_glContext != 0)
            {
                _ = SDL.GLDestroyContext(_glContext);
                _glContext = 0;
            }
            if (Window != 0)
            {
                SDL.DestroyWindow(Window);
                Window = 0;
            }
        }

        private unsafe void Initialize(string title)
        {
            SDL.GLResetAttributes();
            Check(SDL.GLSetAttribute(SDL.GLAttr.ContextMajorVersion, 3));
            Check(SDL.GLSetAttribute(SDL.GLAttr.ContextMinorVersion, 2));
            Check(SDL.GLSetAttribute(SDL.GLAttr.ContextProfileMask, (int)SDL.GLProfile.Core));
            Check(SDL.GLSetAttribute(SDL.GLAttr.DoubleBuffer, 1));
            Check(SDL.GLSetAttribute(SDL.GLAttr.RedSize, 8));
            Check(SDL.GLSetAttribute(SDL.GLAttr.GreenSize, 8));
            Check(SDL.GLSetAttribute(SDL.GLAttr.BlueSize, 8));
            Check(SDL.GLSetAttribute(SDL.GLAttr.AlphaSize, 8));
            Check(SDL.GLSetAttribute(SDL.GLAttr.StencilSize, 8));
            Window = SDL.CreateWindow(title, 800, 600,
                SDL.WindowFlags.OpenGL | SDL.WindowFlags.Resizable | SDL.WindowFlags.HighPixelDensity | SDL.WindowFlags.Hidden);
            if (Window == 0)
            {
                throw new InvalidOperationException($"SDL could not create the window: {SDL.GetError()}");
            }
            _glContext = SDL.GLCreateContext(Window);
            if (_glContext == 0)
            {
                throw new InvalidOperationException($"SDL could not create a GL context: {SDL.GetError()}");
            }
            Check(SDL.GLMakeCurrent(Window, _glContext));
            nint getIntegerv = SDL.GLGetProcAddress("glGetIntegerv");
            if (getIntegerv == 0)
            {
                throw new InvalidOperationException("glGetIntegerv is unavailable.");
            }
            // The window's own framebuffer is not always 0 (macOS layers it), so ask rather than assume.
            int framebuffer = 0;
            ((delegate* unmanaged[Cdecl]<uint, int*, void>)getIntegerv)(GlFramebufferBinding, &framebuffer);
            _framebuffer = (uint)framebuffer;
            _ = SDL.GLSetSwapInterval(1);
            _glInterface = GRGlInterface.Create(SDL.GLGetProcAddress)
                ?? throw new InvalidOperationException("Skia could not resolve the GL functions.");
            _grContext = GRContext.CreateGl(_glInterface)
                ?? throw new InvalidOperationException("Skia could not create a GL context.");
        }

        private void CreateSurface(int width, int height)
        {
            _grContext.Flush(submit: true, synchronous: true);
            ReleaseSurface();
            Check(SDL.GLGetAttribute(SDL.GLAttr.StencilSize, out int stencil));
            Check(SDL.GLGetAttribute(SDL.GLAttr.MultisampleSamples, out int samples));
            _grContext.ResetContext();
            _target = new GRBackendRenderTarget(width, height, samples, stencil, new GRGlFramebufferInfo(_framebuffer, GlRgba8));
            _surface = SKSurface.Create(_grContext, _target, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888)
                ?? throw new InvalidOperationException("Skia could not wrap the window's framebuffer.");
            Width = width;
            Height = height;
        }

        private void ReleaseSurface()
        {
            _surface?.Dispose();
            _surface = null;
            _target?.Dispose();
            _target = null;
        }

        private static void Check(bool success)
        {
            if (!success)
            {
                throw new InvalidOperationException(SDL.GetError());
            }
        }
    }
}
