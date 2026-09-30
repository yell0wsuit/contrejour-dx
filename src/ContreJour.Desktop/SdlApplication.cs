using System;
using System.Diagnostics;
using System.Threading;

using ContreJour.Desktop.Platform;
using ContreJour.Desktop.Platform.Diagnostics;
using ContreJour.Desktop.Platform.Graphics;

using Microsoft.Extensions.Logging;

using Mokus2D;
using Mokus2D.FileSystem;
using Mokus2D.Game;
using Mokus2D.Rendering.Skia;
using Mokus2D.Sound;

using SDL3;

using SkiaSharp;

namespace ContreJour.Desktop
{
    // Runs a game in an SDL window drawn with Skia over GL. The loop makes the calls MonoGame's
    // variable-time-step loop made, in the same order: one zero-time update before the first frame,
    // then events, an update with the real elapsed time, and a draw per frame; vsync paces it.
    public sealed class SdlApplication<T>(IAudioBackend audio, DesktopOptions options) : IDisposable where T : Mokus2DGame, new()
    {
        // MonoGame's Game.InactiveSleepTime default: an unfocused game keeps running, slowly.
        private const int InactiveSleepMilliseconds = 20;

        private const SDL.InitFlags Subsystems = SDL.InitFlags.Video | SDL.InitFlags.Gamepad;

        private readonly IAudioBackend _audio = audio;

        private readonly DesktopOptions _options = options;

        private int _presentedFrames;

        private readonly T _game = new();

        private bool _sdlStarted;

        private bool _disposed;

        private CandidateLifetime _deviceLifetime;

        private SdlGlDevice _device;

        private SdlGameHost _host;

        private SdlInputState _input;

        private SdlGamepads _gamepads;

        private SkiaRenderer _renderer;

        private ApplicationController _applicationController;

        public void Run()
        {
            Start();
            Stopwatch clock = Stopwatch.StartNew();
            TimeSpan previous = TimeSpan.Zero;
            _game.Update(0f);
            while (!_host.QuitRequested)
            {
                PumpEvents();
                if (!_host.IsActive)
                {
                    Thread.Sleep(InactiveSleepMilliseconds);
                }
                TimeSpan now = clock.Elapsed;
                _game.Update((float)(now - previous).TotalSeconds);
                previous = now;
                DrawFrame();
                if (_options.QuitAfterFrames is int limit && _presentedFrames >= limit && !_host.QuitRequested)
                {
                    ILogger logger = Log.For(LogCategories.Host);
                    HostLog.QuittingAfterFrames(logger, limit);
                    _host.Quit();
                }
            }
            _game.OnExiting();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            // Textures go before the GL context they were uploaded to, and the window before SDL.
            _applicationController?.Dispose();
            _renderer?.Dispose();
            _gamepads?.Dispose();
            _deviceLifetime?.Dispose();
            if (_sdlStarted)
            {
                // Only what this host started: the audio backend owns and closes its own subsystem.
                SDL.QuitSubSystem(Subsystems);
            }
        }

        private void Start()
        {
            if (!SDL.InitSubSystem(Subsystems))
            {
                throw new InvalidOperationException($"SDL could not start: {SDL.GetError()}");
            }
            _sdlStarted = true;
            _deviceLifetime = new CandidateLifetime();
            SdlGlDevice gl = _deviceLifetime.Own(new SdlGlDevice(static _ => { }));
            gl.Initialize();
            _device = gl;
            _ = SDL.SetWindowTitle(_device.Window, SdlGraphicsDevice.TitleFor(_device.Kind));
            _host = new SdlGameHost(_device.Window);
            _input = new SdlInputState(() => _host.Letterbox, _host.WarpMouse);
            _gamepads = new SdlGamepads(
                static () => SDL.GetGamepads(out _) ?? [],
                static id => SDL.OpenGamepad(id),
                static handle => SDL.CloseGamepad(handle));
            _gamepads.OpenConnected();
            _renderer = new SkiaRenderer();
            _applicationController = new ApplicationController(_host, _input, new FileLoader(), _audio, _renderer);
            _game.Initialize(_applicationController);
            // Subscribed after the game is set up, as the MonoGame host did: resizes during
            // Initialize would otherwise reach the game before its views exist.
            _host.ClientSizeChanged += OnClientSizeChanged;
            _host.ActiveChanged += OnActiveChanged;
        }

        private void PumpEvents()
        {
            while (SDL.PollEvent(out SDL.Event e))
            {
                _gamepads.HandleEvent(e);
                _input.HandleEvent(e);
                _host.HandleEvent(e);
            }
        }

        private void DrawFrame()
        {
            if (!_device.AcquireFrame())
            {
                return;
            }
            SKCanvas canvas = _device.Canvas;
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
            _device.Flush();
            _device.Present();
            _presentedFrames++;
        }

        private void OnClientSizeChanged()
        {
            _game.OnApplicationViewChanged(EventArgs.Empty);
        }

        private void OnActiveChanged(bool active)
        {
            if (active)
            {
                _game.OnActivated();
            }
            else
            {
                _game.OnDeactivated();
            }
        }
    }
}
