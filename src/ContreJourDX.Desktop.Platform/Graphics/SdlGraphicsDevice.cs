using System;
using System.Threading;

using SDL3;

using SkiaSharp;

namespace ContreJourDX.Desktop.Platform.Graphics
{
    // An SDL window and the Skia surface a frame is drawn into (from cuttherope-dx's
    // SdlGraphicsDevice). Each device owns its own window, since SDL fixes a window's graphics API
    // when it is created. Everything acquired is registered with a CandidateLifetime right after it
    // succeeds, so a failure part-way through Initialize releases exactly what was acquired. Main
    // thread only.
    public abstract class SdlGraphicsDevice : IDisposable
    {
        public const string WindowTitle = "Contre Jour DX";

        private readonly int _ownerThread = Environment.CurrentManagedThreadId;

        private readonly CandidateLifetime _resources = new();

        private SKSurface _surface;

        private GRBackendRenderTarget _target;

        private bool _disposed;

        // fault is called at the named points after-device, before-surface and after-surface, and
        // may throw there; see FaultPlan.
        protected SdlGraphicsDevice(Action<string> fault)
        {
            ArgumentNullException.ThrowIfNull(fault);
            Fault = fault;
        }

        public abstract GraphicsBackendKind Kind { get; }

        public nint Window { get; private set; }

        // Null for a device that draws on the CPU.
        public GRContext Context { get; protected set; }

        public SKCanvas Canvas => _surface?.Canvas ?? throw new InvalidOperationException("No frame is acquired.");

        // Pixel size of the current surface.
        public int Width { get; protected set; }

        public int Height { get; protected set; }

        protected Action<string> Fault { get; }

        protected bool HasFrame => _surface != null;

        public static string TitleFor(GraphicsBackendKind kind, string version)
        {
            return $"{WindowTitle} v{version} | {kind}";
        }

        // Creates the hidden window and everything the backend needs to draw into it.
        public abstract void Initialize();

        // False while the window is minimized or has no area, when there is nothing to draw into.
        public abstract bool AcquireFrame();

        public abstract void Present();

        // Submits the frame and waits for it. Skia abandons a context when the driver reports the
        // device gone, and every later draw would be dropped silently.
        public void Flush()
        {
            CheckThread();
            Context?.Flush(submit: true, synchronous: true);
            if (Context?.IsAbandoned == true)
            {
                throw new GraphicsDeviceLostException("Skia abandoned the graphics context.");
            }
        }

        // The acquired frame's pixels, for the startup draw check.
        public SKBitmap ReadPixels()
        {
            CheckThread();
            if (_surface == null)
            {
                throw new InvalidOperationException("No frame is acquired.");
            }
            SKBitmap bitmap = new(new SKImageInfo(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul));
            if (!_surface.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0))
            {
                bitmap.Dispose();
                throw new InvalidOperationException("The frame could not be read back.");
            }
            return bitmap;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
            {
                return;
            }
            CheckThread();
            _disposed = true;
            try
            {
                ClearSurface();
            }
            finally
            {
                _resources.Dispose();
            }
        }

        protected T Own<T>(T resource) where T : IDisposable
        {
            return _resources.Own(resource);
        }

        protected void Own(Action release)
        {
            _ = _resources.Own(new NativeRelease(release));
        }

        // Hidden: candidates are built and thrown away until one passes its draw check, and the host
        // sizes, places and shows the survivor.
        protected void CreateWindow(SDL.WindowFlags flags)
        {
            CheckThread();
            Window = SDL.CreateWindow(WindowTitle, 800, 600,
                flags | SDL.WindowFlags.Resizable | SDL.WindowFlags.HighPixelDensity | SDL.WindowFlags.Hidden);
            if (Window == 0)
            {
                throw new InvalidOperationException($"SDL could not create the window: {SDL.GetError()}");
            }
            Own(() =>
            {
                SDL.DestroyWindow(Window);
                Window = 0;
            });
        }

        protected void CheckThread()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (Environment.CurrentManagedThreadId != _ownerThread || !SDL.IsMainThread())
            {
                throw new InvalidOperationException("SDL graphics must run on the main thread.");
            }
        }

        protected bool GetDrawableSize(out int width, out int height)
        {
            CheckThread();
            Check(SDL.GetWindowSizeInPixels(Window, out width, out height));
            return width > 0 && height > 0 && (SDL.GetWindowFlags(Window) & SDL.WindowFlags.Minimized) == 0;
        }

        // Keeps the target even when Skia refuses it, so ClearSurface still releases it.
        protected void SetSurface(GRBackendRenderTarget target, GRSurfaceOrigin origin, SKColorType colorType)
        {
            ClearSurface();
            _target = target;
            _surface = SKSurface.Create(Context, _target, origin, colorType)
                ?? throw new InvalidOperationException("Skia could not wrap the window's render target.");
        }

        protected void SetRasterSurface(SKSurface raster)
        {
            ClearSurface();
            _surface = raster ?? throw new InvalidOperationException("Skia could not create a raster surface.");
        }

        // The surface goes before its backing target, and both before the device.
        protected void ClearSurface()
        {
            _surface?.Dispose();
            _surface = null;
            _target?.Dispose();
            _target = null;
        }

        protected static void Check(bool success)
        {
            if (!success)
            {
                throw new InvalidOperationException(SDL.GetError());
            }
        }

        private sealed class NativeRelease(Action release) : IDisposable
        {
            private Action _pending = release;

            public void Dispose()
            {
                Interlocked.Exchange(ref _pending, null)?.Invoke();
            }
        }
    }
}
