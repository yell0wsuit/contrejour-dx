using System;
using System.Runtime.InteropServices;


using Microsoft.Extensions.Logging;

using Mokus2D.Diagnostics;

using SDL3;

using SkiaSharp;

namespace ContreJourDX.Desktop.Platform.Graphics
{
    // A GL context with a Skia surface over the window's default framebuffer: core 3.2 from the system
    // driver, or OpenGL ES from ANGLE's libraries (trimmed from cuttherope-dx's SdlGlDevice).
    public sealed class SdlGlDevice(Action<string> fault, GlContextProfile profile) : SdlGraphicsDevice(fault)
    {
        private const uint GlFramebufferBinding = 0x8CA6;

        private const uint GlRgba8 = 0x8058;

        private const uint GlVendor = 0x1F00;

        private const uint GlRenderer = 0x1F01;

        private const uint GlVersion = 0x1F02;

        private readonly GlContextProfile _profile = profile ?? throw new ArgumentNullException(nameof(profile));

        private nint _glContext;

        private uint _framebuffer;

        public override GraphicsBackendKind Kind => _profile.UsesAngle ? GraphicsBackendKind.Angle : GraphicsBackendKind.OpenGL;

        // Forcing EGL does the work. Windows has two GL loaders behind the same request, and the
        // default one opens the named library as if it were the system opengl32 and looks for wgl entry
        // points an ES library does not export. The driver hint is the older half of the same choice,
        // kept because SDL consults it when picking between the two. Both libraries are named because
        // each loader reads its own: one opens EGL, the other the client GL it dispatches through.
        internal static void ApplyAngleHints(GlContextProfile profile, GlHintScope hints)
        {
            hints.Set(SDL.Hints.VideoForceEGL, "1");
            hints.Set(SDL.Hints.OpenGLESDriver, "1");
            hints.Set(SDL.Hints.EGLLibrary, profile.EglLibrary);
            hints.Set(SDL.Hints.OpenGLLibrary, profile.GlesLibrary);
        }

        // Every attempt rebuilds SDL's video subsystem, not only ANGLE's. The loader hints are read
        // once, when the video device is built, and an ANGLE attempt that came up and was then rejected
        // leaves that device bound to EGL: native GL tried next would open ANGLE's libraries as its own.
        // The hints only need to hold while the context is made.
        public override void Initialize()
        {
            CheckThread();
            using GlHintScope hints = new();
            try
            {
                RecycleVideo(_profile.UsesAngle ? () => ApplyAngleHints(_profile, hints) : null);
                CreateContext();
            }
            catch
            {
                // The window and context go while the video device that made them still exists, then
                // the hints, then the subsystem is rebuilt without them for whatever is tried next.
                Dispose();
                hints.Dispose();
                try
                {
                    RecycleVideo(null);
                }
                catch (InvalidOperationException)
                {
                    // Why this candidate failed is the useful half; a subsystem that will not come back
                    // says so again on the next candidate.
                }
                throw;
            }
        }

        public override bool AcquireFrame()
        {
            if (!GetDrawableSize(out int width, out int height))
            {
                return false;
            }
            Check(SDL.GLMakeCurrent(Window, _glContext));
            if (!HasFrame || width != Width || height != Height)
            {
                Fault("before-surface");
                CreateSurface(width, height);
                Fault("after-surface");
            }
            return true;
        }

        public override void Present()
        {
            CheckThread();
            if (!SDL.GLSwapWindow(Window))
            {
                throw SwapFailed(SDL.GetError());
            }
        }

        // A swap that fails means the context is no longer usable, which on GL (and ANGLE over a reset
        // Direct3D device) is what a driver reset looks like. Skia notices the same loss, but only on
        // the frame after this one.
        internal static GraphicsDeviceLostException SwapFailed(string error)
        {
            return new GraphicsDeviceLostException($"SDL could not swap the GL window: {error}");
        }

        // On ANGLE, SDL chooses the EGL config while it makes the window, so a version the hardware does
        // not offer is refused there rather than at the context: ES 3.0 on Direct3D feature level 10_0.
        internal static GlContextRefusedException WindowRefused(GlContextProfile profile, Exception failure)
        {
            return new GlContextRefusedException(
                $"SDL could not make a window for OpenGL ES {profile.Major}.{profile.Minor}: {failure.Message}", failure);
        }

        // Nothing may hold a window across this: candidates are built one at a time, and the one before
        // (or the lost device, during recovery) is released before the next starts.
        private static void RecycleVideo(Action apply)
        {
            SDL.QuitSubSystem(SDL.InitFlags.Video);
            apply?.Invoke();
            if (!SDL.InitSubSystem(SDL.InitFlags.Video))
            {
                throw new InvalidOperationException($"SDL could not restart its video subsystem: {SDL.GetError()}");
            }
        }

        private void CreateContext()
        {
            SDL.GLResetAttributes();
            Check(SDL.GLSetAttribute(SDL.GLAttr.ContextMajorVersion, _profile.Major));
            Check(SDL.GLSetAttribute(SDL.GLAttr.ContextMinorVersion, _profile.Minor));
            Check(SDL.GLSetAttribute(SDL.GLAttr.ContextProfileMask, _profile.ProfileMask));
            Check(SDL.GLSetAttribute(SDL.GLAttr.DoubleBuffer, 1));
            Check(SDL.GLSetAttribute(SDL.GLAttr.RedSize, 8));
            Check(SDL.GLSetAttribute(SDL.GLAttr.GreenSize, 8));
            Check(SDL.GLSetAttribute(SDL.GLAttr.BlueSize, 8));
            Check(SDL.GLSetAttribute(SDL.GLAttr.AlphaSize, 8));
            Check(SDL.GLSetAttribute(SDL.GLAttr.StencilSize, 8));
            try
            {
                CreateWindow(SDL.WindowFlags.OpenGL);
            }
            catch (InvalidOperationException failure) when (_profile.UsesAngle)
            {
                throw WindowRefused(_profile, failure);
            }
            _glContext = SDL.GLCreateContext(Window);
            if (_glContext == 0)
            {
                throw new GlContextRefusedException(
                    $"SDL could not create a GL {_profile.Major}.{_profile.Minor} context: {SDL.GetError()}");
            }
            nint glContext = _glContext;
            Own(() => SDL.GLDestroyContext(glContext));
            Check(SDL.GLMakeCurrent(Window, _glContext));
            // The window's own framebuffer is not always 0 (macOS layers it), so ask rather than assume.
            _framebuffer = (uint)GetInteger(GlFramebufferBinding);
            _ = SDL.GLSetSwapInterval(1);
            GRGlInterface glInterface = Own(GRGlInterface.Create(SDL.GLGetProcAddress)
                ?? throw new InvalidOperationException("Skia could not resolve the GL functions."));
            Context = Own(GRContext.CreateGl(glInterface)
                ?? throw new InvalidOperationException("Skia could not create a GL context."));
            ILogger logger = Log.For(LogCategories.Graphics);
            if (logger.IsEnabled(LogLevel.Information))
            {
                string name = GetString(GlRenderer);
                string version = $"{GetString(GlVersion)} ({GetString(GlVendor)})";
                GraphicsDeviceLog.Adapter(logger, Kind, name, version);
            }
            Fault("after-device");
        }

        private void CreateSurface(int width, int height)
        {
            Flush();
            ClearSurface();
            Check(SDL.GLGetAttribute(SDL.GLAttr.StencilSize, out int stencil));
            Check(SDL.GLGetAttribute(SDL.GLAttr.MultisampleSamples, out int samples));
            Context.ResetContext();
            SetSurface(new GRBackendRenderTarget(width, height, samples, stencil, new GRGlFramebufferInfo(_framebuffer, GlRgba8)),
                GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
            Width = width;
            Height = height;
        }

        private static unsafe int GetInteger(uint name)
        {
            nint getIntegerv = SDL.GLGetProcAddress("glGetIntegerv");
            if (getIntegerv == 0)
            {
                throw new InvalidOperationException("glGetIntegerv is unavailable.");
            }
            int value = 0;
            ((delegate* unmanaged[Cdecl]<uint, int*, void>)getIntegerv)(name, &value);
            return value;
        }

        private static unsafe string GetString(uint name)
        {
            nint getString = SDL.GLGetProcAddress("glGetString");
            if (getString == 0)
            {
                return "unknown";
            }
            byte* text = ((delegate* unmanaged[Cdecl]<uint, byte*>)getString)(name);
            return Marshal.PtrToStringUTF8((nint)text) ?? "unknown";
        }
    }
}
