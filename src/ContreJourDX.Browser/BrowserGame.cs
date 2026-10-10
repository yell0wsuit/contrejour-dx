using System;
using System.Numerics;

using ContreJourDX.Browser.Platform;
using ContreJourDX.Saving;

using Microsoft.Extensions.Logging;

using Mokus2D.Content;
using Mokus2D.Diagnostics;
using Mokus2D.FileSystem;
using Mokus2D.Game;
using Mokus2D.Rendering.Skia;
using Mokus2D.Sound;
using Mokus2D.Util.Data;

using SkiaSharp;

namespace ContreJourDX.Browser
{
    // The running game, built when the player presses Play. It makes the calls the desktop loop makes, in the
    // same order: one zero-time update before the first frame, then per frame an update with the real elapsed
    // time (Mokus2DGame clamps it) and a draw.
    internal sealed class BrowserGame : IDisposable
    {
        public static readonly ContentFormats Formats = new(".webp", ".ogg", ".ogg");

        private const int RunningMarkerFrame = 120;

        private readonly ContreJourDXApplication _game = new();

        private readonly BrowserGameHost _host;

        private readonly SkiaRenderer _renderer = new();

        private readonly SkiaSurface _surface;

        private readonly IAudioBackend _audio;

        private readonly FrameClock _clock = new();

        private bool _active = true;

        private bool _lost;

        private int _frames;

        public BrowserGame(SkiaSurface surface, IFileLoader files, IAudioBackend audio, Vector2 cssSize, Vector2 pixelSize, double timestampMs)
        {
            _surface = surface;
            _audio = audio;
            double[] screen = PageInterop.ScreenSize();
            Point canvas = BrowserCanvas.LogicalSize(screen[0], screen[1], screen[2]);
            _host = new BrowserGameHost(canvas, cssSize, pixelSize);
            Input = new BrowserInputState(() => _host.Letterbox);
            ApplicationController controller = new(_host, Input, files, audio, _renderer, Formats);
            _game.Initialize(controller);
            _game.Update(0f);
            _ = _clock.Advance(timestampMs);
            ILogger gameStartedLogger = Log.For(LogCategories.Host);
            BrowserLog.GameStarted(gameStartedLogger, canvas.X, canvas.Y);
        }

        public BrowserInputState Input { get; }

        public void Frame(double timestampMs)
        {
            if (_lost)
            {
                return;
            }
            // Stepped while away too, so a page that keeps getting frames owes no time on return.
            float elapsed = _clock.Advance(timestampMs);
            if (_active)
            {
                _game.Update(elapsed);
            }
            Draw();
            if (++_frames == RunningMarkerFrame)
            {
                ILogger runningLogger = Log.For(LogCategories.Host);
                BrowserLog.Running(runningLogger, RunningMarkerFrame);
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
                // A hidden page got no frames: the time away is not owed, as on desktop.
                _clock.Reset();
                SetAudioSuspended(false);
                _game.OnActivated();
            }
            else
            {
                Input.ReleaseAll();
                _game.OnDeactivated();
                SetAudioSuspended(true);
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
            ILogger contextLostLogger = Log.For(LogCategories.Graphics);
            BrowserLog.ContextLost(contextLostLogger);
        }

        public void Dispose()
        {
            _game.Dispose();
            _renderer.Dispose();
        }

        private void SetAudioSuspended(bool suspended)
        {
            if (_audio is WebAudioBackend web)
            {
                web.Suspended = suspended;
            }
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
