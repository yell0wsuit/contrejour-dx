using System;
using System.Numerics;

using ContreJour.Browser.Platform;
using ContreJour.Saving;

using Mokus2D.Content;
using Mokus2D.FileSystem;
using Mokus2D.Game;
using Mokus2D.Rendering.Skia;
using Mokus2D.Sound;
using Mokus2D.Util.Data;

using SkiaSharp;

namespace ContreJour.Browser
{
    // The running game, built when the player presses Play. It makes the calls the desktop loop makes, in the
    // same order: one zero-time update before the first frame, then per frame an update with the real elapsed
    // time (Mokus2DGame clamps it) and a draw.
    internal sealed class BrowserGame : IDisposable
    {
        public static readonly ContentFormats Formats = new(".webp", ".ogg", ".ogg");

        private const int RunningMarkerFrame = 120;

        private readonly ContreJourApplication _game = new();

        private readonly BrowserGameHost _host;

        private readonly SkiaRenderer _renderer = new();

        private readonly SkiaSurface _surface;

        private double _previousMs;

        private bool _active = true;

        private bool _lost;

        private int _frames;

        public BrowserGame(SkiaSurface surface, IFileLoader files, IAudioBackend audio, Vector2 cssSize, Vector2 pixelSize, double timestampMs)
        {
            _surface = surface;
            double[] screen = PageInterop.ScreenSize();
            Point canvas = BrowserCanvas.LogicalSize(screen[0], screen[1], screen[2]);
            _host = new BrowserGameHost(canvas, cssSize, pixelSize);
            Input = new BrowserInputState(() => _host.Letterbox);
            ApplicationController controller = new(_host, Input, files, audio, _renderer, Formats);
            _game.Initialize(controller);
            _game.Update(0f);
            _previousMs = timestampMs;
            Console.WriteLine($"cj-game-started: canvas {canvas.X}x{canvas.Y}");
        }

        public BrowserInputState Input { get; }

        public void Frame(double timestampMs)
        {
            if (_lost)
            {
                return;
            }
            if (_active)
            {
                _game.Update((float)((timestampMs - _previousMs) / 1000.0));
            }
            // While away the clock keeps up, so no time is owed on return.
            _previousMs = timestampMs;
            Draw();
            if (++_frames == RunningMarkerFrame)
            {
                Console.WriteLine($"cj-running: {RunningMarkerFrame} frames");
            }
        }

        // Mirrors the desktop host's focus handling, plus a forced save: a page can be discarded while away.
        public void SetActive(bool active)
        {
            if (active == _active)
            {
                return;
            }
            _active = active;
            _host.IsActive = active;
            if (active)
            {
                _game.OnActivated();
            }
            else
            {
                Input.ReleaseAll();
                _game.OnDeactivated();
                Preferences.RequestSave();
                Preferences.Update(force: true);
            }
        }

        public void Resize(Vector2 cssSize, Vector2 pixelSize)
        {
            _host.Resize(cssSize, pixelSize);
        }

        // No frame draws again; the page tells the player to reload. Saved now, since nothing else will run.
        public void OnContextLost()
        {
            if (_lost)
            {
                return;
            }
            _lost = true;
            Preferences.RequestSave();
            Preferences.Update(force: true);
            Console.WriteLine("cj-context-lost: reload required");
        }

        public void Dispose()
        {
            _game.Dispose();
            _renderer.Dispose();
        }

        private void Draw()
        {
            SKCanvas canvas = _surface.Canvas;
            Letterbox letterbox = _host.Letterbox;
            canvas.ResetMatrix();
            canvas.Clear(SKColors.Black);
            int saved = canvas.Save();
            canvas.Translate(letterbox.Offset.X, letterbox.Offset.Y);
            canvas.Scale(letterbox.Scale);
            canvas.ClipRect(new SKRect(0, 0, letterbox.LogicalSize.X, letterbox.LogicalSize.Y));
            Mokus2D.Graphics.Color background = _game.BackgroundColor;
            canvas.Clear(new SKColor(background.R, background.G, background.B, background.A));
            _renderer.SetTarget(canvas, _host.BackBufferSize.X, _host.BackBufferSize.Y);
            _game.Draw();
            canvas.RestoreToCount(saved);
            _surface.Flush();
        }
    }
}
