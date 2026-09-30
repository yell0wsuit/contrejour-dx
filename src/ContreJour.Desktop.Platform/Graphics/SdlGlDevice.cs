using System;
using System.Runtime.InteropServices;

using ContreJour.Desktop.Platform.Diagnostics;

using Microsoft.Extensions.Logging;

using SDL3;

using SkiaSharp;

namespace ContreJour.Desktop.Platform.Graphics
{
    // An OpenGL 3.2 core context with a Skia surface over the window's default framebuffer (trimmed
    // from cuttherope-dx's SdlGlDevice).
    public sealed class SdlGlDevice(Action<string> fault) : SdlGraphicsDevice(fault)
    {
        private const uint GlFramebufferBinding = 0x8CA6;

        private const uint GlRgba8 = 0x8058;

        private const uint GlVendor = 0x1F00;

        private const uint GlRenderer = 0x1F01;

        private const uint GlVersion = 0x1F02;

        private nint _glContext;

        private uint _framebuffer;

        public override GraphicsBackendKind Kind => GraphicsBackendKind.OpenGL;

        public override void Initialize()
        {
            CheckThread();
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
            CreateWindow(SDL.WindowFlags.OpenGL);
            _glContext = SDL.GLCreateContext(Window);
            if (_glContext == 0)
            {
                throw new InvalidOperationException($"SDL could not create a GL context: {SDL.GetError()}");
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
                throw new InvalidOperationException($"SDL could not swap the GL window: {SDL.GetError()}");
            }
        }

        private void CreateSurface(int width, int height)
        {
            Context.Flush(submit: true, synchronous: true);
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
