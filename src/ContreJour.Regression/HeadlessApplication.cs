using System;

using Mokus2D;
using Mokus2D.Content;
using Mokus2D.FileSystem;
using Mokus2D.Game;
using Mokus2D.Rendering.Skia;
using Mokus2D.Sound;

using SkiaSharp;

namespace ContreJour.Regression
{
    // Runs a game with no window, drawing into a CPU surface the size of the back buffer. The loop
    // makes the calls MonoGame's variable-time-step loop makes, in the same order: one update before
    // the first frame, then an update and a draw per frame, the frame that quits included.
    internal sealed class HeadlessApplication<T> : IDisposable where T : Mokus2DGame, new()
    {
        // The harness steps its own fixed time and ignores this value.
        private const float FrameTime = 1f / 60f;

        private readonly T _game;

        private readonly HeadlessHost _host = new();

        private readonly SkiaRenderer _renderer = new();

        private readonly ApplicationController _applicationController;

        private SKSurface _surface;

        public HeadlessApplication(IAudioBackend audio)
        {
            _game = new T();
            _applicationController = new ApplicationController(_host, new NullInputSource(), new FileLoader(), audio, _renderer, ContentFormats.Desktop);
        }

        public void Run()
        {
            _game.Initialize(_applicationController);
            _surface = SKSurface.Create(new SKImageInfo(_host.BackBufferSize.X, _host.BackBufferSize.Y, SKColorType.Rgba8888, SKAlphaType.Premul))
                ?? throw new InvalidOperationException("Could not create the headless surface.");
            _host.LockSize();
            _renderer.SetTarget(_surface.Canvas, _host.BackBufferSize.X, _host.BackBufferSize.Y);
            _game.Update(FrameTime);
            while (!_host.QuitRequested)
            {
                _game.Update(FrameTime);
                DrawFrame();
            }
            _game.OnExiting();
        }

        // Draws one frame as the loop does and reads it back as RGBA bytes, for the pixel check.
        public (byte[] Pixels, int Width, int Height) CaptureFrame()
        {
            DrawFrame();
            using SKPixmap pixmap = _surface.PeekPixels();
            return (pixmap.GetPixelSpan().ToArray(), pixmap.Width, pixmap.Height);
        }

        public void Dispose()
        {
            _applicationController.Dispose();
            _renderer.Dispose();
            _surface?.Dispose();
        }

        private void DrawFrame()
        {
            Mokus2D.Graphics.Color background = _game.BackgroundColor;
            _surface.Canvas.Clear(new SKColor(background.R, background.G, background.B, background.A));
            _game.Draw();
        }
    }
}
