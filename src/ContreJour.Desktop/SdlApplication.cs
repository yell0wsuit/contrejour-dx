using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using ContreJour.Desktop.Platform;
using ContreJour.Desktop.Platform.Audio;
using ContreJour.Desktop.Platform.Diagnostics;
using ContreJour.Desktop.Platform.Graphics;

using Microsoft.Extensions.Logging;

using Mokus2D;
using Mokus2D.Content;
using Mokus2D.Diagnostics;
using Mokus2D.FileSystem;
using Mokus2D.Game;
using Mokus2D.Rendering.Skia;
using Mokus2D.Sound;

using SDL3;

using SkiaSharp;

namespace ContreJour.Desktop
{
    // Runs a game in an SDL window drawn by Skia on the first renderer that passes a draw check. The
    // loop makes the calls MonoGame's variable-time-step loop made, in the same order: one zero-time
    // update before the first frame, then events, an update with the real elapsed time, and a draw
    // per frame; vsync paces it.
    public sealed class SdlApplication<T>(IAudioBackend audio, DesktopOptions options, string saveDirectory) : IDisposable where T : Mokus2DGame, new()
    {
        // How often a frozen game's window is redrawn while it is away.
        private const int InactiveSleepMilliseconds = 20;

        private const SDL.InitFlags Subsystems = SDL.InitFlags.Video | SDL.InitFlags.Gamepad;

        private readonly IAudioBackend _audio = audio;

        private readonly DesktopOptions _options = options;

        private readonly string _saveDirectory = saveDirectory;

        private readonly T _game = new();

        private bool _sdlStarted;

        private bool _disposed;

        private int _presentedFrames;

        private string _platform;

        private GraphicsSelection<SdlGraphicsDevice> _selection;

        private SdlGameHost _host;

        private SdlInputState _input;

        private SdlGamepads _gamepads;

        private SkiaRenderer _renderer;

        private ApplicationController _applicationController;

        private readonly Stopwatch _clock = new();

        private TimeSpan _previous;

        private GraphicsRecoveryCoordinator _recovery;

        private Queue<int> _losses;

        // The graphics device could not be restored and the run ended early.
        public bool Abandoned { get; private set; }

        public void Run()
        {
            Start();
            _clock.Start();
            _game.Update(0f);
            _previous = _clock.Elapsed;
            while (!_host.QuitRequested)
            {
                PumpEvents();
                if (_host.IsActive)
                {
                    TimeSpan now = _clock.Elapsed;
                    _game.Update((float)(now - _previous).TotalSeconds);
                    _previous = now;
                }
                else
                {
                    // Frozen while the window is away, as the Windows 8 game was when the system
                    // suspended it; the clock keeps up, so no time is owed on return. Frames are still
                    // drawn, so resizes and full-screen changes show the paused scene.
                    Thread.Sleep(InactiveSleepMilliseconds);
                    _previous = _clock.Elapsed;
                }
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
            // Textures go before the device that drew them, and the window before SDL.
            _applicationController?.Dispose();
            _renderer?.Dispose();
            _gamepads?.Dispose();
            _selection?.Dispose();
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
            _platform = BackendSelector.CurrentPlatform();
            _recovery = new GraphicsRecoveryCoordinator(_platform, _options.Renderer);
            _losses = new Queue<int>(_options.DeviceLosses);
            _selection = SelectDevice();
            SdlGraphicsDevice device = _selection.Device;
            _ = SDL.SetWindowTitle(device.Window, SdlGraphicsDevice.TitleFor(device.Kind));
            ApplyWindowIcon(device.Window);
            ILogger logger = Log.For(LogCategories.Host);
            string audioState = _audio is NullAudioBackend ? "off" : "on";
            HostLog.Renderer(logger, _selection.Kind, audioState);
            foreach (RendererFailure failure in _selection.Failures)
            {
                HostLog.RejectedRenderer(logger, failure.Kind, failure.Failure.Message);
            }
            _host = new SdlGameHost(device.Window);
            _input = new SdlInputState(() => _host.Letterbox, _host.WarpMouse);
            _gamepads = new SdlGamepads(
                static () => SDL.GetGamepads(out _) ?? [],
                static id => SDL.OpenGamepad(id),
                static handle => SDL.CloseGamepad(handle));
            _gamepads.OpenConnected();
            _renderer = new SkiaRenderer();
            _applicationController = new ApplicationController(_host, _input, new FileLoader(), _audio, _renderer, ContentFormats.Desktop);
            _game.Initialize(_applicationController);
            // Subscribed after the game is set up, as the MonoGame host did: resizes during
            // Initialize would otherwise reach the game before its views exist.
            _host.ClientSizeChanged += OnClientSizeChanged;
            _host.ActiveChanged += OnActiveChanged;
            if (!_host.IsActive)
            {
                OnActiveChanged(false);
            }
        }

        // A driver that crashes the process while starting leaves RendererMemory's marker behind,
        // and the next launch skips that renderer.
        private GraphicsSelection<SdlGraphicsDevice> SelectDevice()
        {
            RendererMemory memory = new(Path.Combine(_saveDirectory, RendererMemory.FileName));
            GraphicsBackendKind[] order = memory.Filter(BackendSelector.PreferenceOrder(_platform, _options.Renderer));
            if (memory.Blamed is GraphicsBackendKind blamed && Array.IndexOf(order, blamed) < 0)
            {
                ILogger logger = Log.For(LogCategories.Host);
                HostLog.SkippingBlamedRenderer(logger, blamed);
            }
            GraphicsSelection<SdlGraphicsDevice> selection = BackendSelector.Attempt(order, (kind, lifetime) =>
            {
                memory.BeginAttempt(kind);
                return CreateDevice(kind, lifetime);
            }, ValidateDevice, memory.Absolve);
            memory.RecordSuccess();
            return selection;
        }

        private SdlGraphicsDevice CreateDevice(GraphicsBackendKind kind, CandidateLifetime lifetime)
        {
            Action<string> fault = _options.Faults.For(kind);
            if (kind == GraphicsBackendKind.Angle)
            {
                return CreateAngleDevice(fault, lifetime);
            }
            SdlGraphicsDevice device = kind == GraphicsBackendKind.Vulkan
                ? new VulkanDevice(fault)
                : kind == GraphicsBackendKind.OpenGL
                ? new SdlGlDevice(fault, GlContextProfile.DesktopCore)
                : kind == GraphicsBackendKind.Metal
                ? new MetalDevice(fault)
                : kind == GraphicsBackendKind.Software
                ? new SdlSoftwareDevice(fault)
                : throw new PlatformNotSupportedException($"The {kind} renderer is not available.");
            _ = lifetime.Own(device);
            device.Initialize();
            return device;
        }

        // From the libraries shipped in angle/ beside the game; a build without them fails here, before
        // SDL is touched. Both attempts share one fault hook, so a fault fires once.
        private static SdlGlDevice CreateAngleDevice(Action<string> fault, CandidateLifetime lifetime)
        {
            return AngleRuntime.TryLocate(AppContext.BaseDirectory, out string egl, out string gles)
                ? GlContextProfile.Angle(egl, gles).Start(profile =>
                {
                    SdlGlDevice device = lifetime.Own(new SdlGlDevice(fault, profile));
                    device.Initialize();
                    return device;
                })
                : throw new PlatformNotSupportedException("The ANGLE libraries are not installed beside the game.");
        }

        // A candidate must draw a shaded frame that reads back before the game gets it: a driver that
        // fails to build a program draws nothing while Skia reports success.
        private static void ValidateDevice(SdlGraphicsDevice device)
        {
            if (!device.AcquireFrame())
            {
                throw new InvalidOperationException("The window has nothing to draw into.");
            }
            DrawCheck.Draw(device.Canvas, device.Width, device.Height);
            device.Flush();
            SKPointI at = DrawCheck.Sample(device.Width, device.Height);
            using (SKBitmap frame = device.ReadPixels())
            {
                SKColor sample = frame.GetPixel(at.X, at.Y);
                if (!DrawCheck.Drew(sample))
                {
                    throw new InvalidOperationException(
                        $"The renderer accepted a shaded draw but read back {sample} at {at.X},{at.Y}, the color it was cleared to.");
                }
            }
            // Presented cleared, so the window does not show the check when the host reveals it.
            device.Canvas.Clear(DrawCheck.Background);
            device.Flush();
            device.Present();
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

        private bool DrawScene()
        {
            SdlGraphicsDevice device = _selection.Device;
            if (!device.AcquireFrame())
            {
                return false;
            }
            SKCanvas canvas = device.Canvas;
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
            device.Flush();
            device.Present();
            return true;
        }

        private void DrawFrame()
        {
            if (Abandoned)
            {
                return;
            }
            if (_losses.Count > 0 && _presentedFrames >= _losses.Peek())
            {
                _ = _losses.Dequeue();
                RecoverDevice(new GraphicsDeviceLostException($"Loss injected at frame {_presentedFrames}."));
                return;
            }
            try
            {
                if (DrawScene())
                {
                    _presentedFrames++;
                    _recovery.FramePresented();
                }
            }
            catch (GraphicsDeviceLostException lost)
            {
                RecoverDevice(lost);
            }
        }

        // The renderer and every texture carry over: textures are CPU images that Skia uploads
        // again into the replacement's context the first time they are drawn.
        private void RecoverDevice(GraphicsDeviceLostException lost)
        {
            ILogger logger = Log.For(LogCategories.Host);
            HostLog.DeviceLost(logger, lost.Message);
            // Even an exhausted recovery must silence audio and clear input before its dialog.
            _host.DetachWindow();
            if (!_recovery.TryBeginRecovery())
            {
                Abandon($"The graphics device was replaced {GraphicsRecoveryCoordinator.MaximumFramelessRecoveries} times without drawing a frame.");
                return;
            }
            try
            {
                _selection = _recovery.Recover(_selection, CreateDevice, ValidateDevice);
            }
            catch (GraphicsRecoveryFailedException failure)
            {
                // Recover released the lost device.
                _selection = null;
                Abandon(failure.Message);
                return;
            }
            SdlGraphicsDevice device = _selection.Device;
            foreach (RendererFailure failure in _selection.Failures)
            {
                HostLog.RejectedRenderer(logger, failure.Kind, failure.Failure.Message);
            }
            _ = SDL.SetWindowTitle(device.Window, SdlGraphicsDevice.TitleFor(device.Kind));
            ApplyWindowIcon(device.Window);
            _host.AttachWindow(device.Window);
            // Building a device takes real time the game should not be asked to catch up on.
            _previous = _clock.Elapsed;
            HostLog.Recovered(logger, device.Kind, _presentedFrames, _recovery.Recoveries);
        }

        // Shows the player why the game has to close, without touching the graphics device.
        private void Abandon(string reason)
        {
            ILogger logger = Log.For(LogCategories.Host);
            HostLog.Abandoning(logger, reason);
            Abandoned = true;
            CrashDialog.Show($"The graphics device could not be restored: {reason}{Environment.NewLine}{Environment.NewLine}The game has to close.");
            _host.Quit();
        }

        private void OnClientSizeChanged()
        {
            _game.OnApplicationViewChanged(EventArgs.Empty);
        }

        private void OnActiveChanged(bool active)
        {
            ILogger logger = Log.For(LogCategories.Host);
            HostLog.FocusChanged(logger, active);
            if (active)
            {
                // Window transitions can spend time inside SDL; none of that is gameplay time.
                _previous = _clock.Elapsed;
                SetAudioSuspended(false);
                _game.OnActivated();
            }
            else
            {
                _input.ReleaseAll();
                _game.OnDeactivated();
                SetAudioSuspended(true);
            }
        }

        // A missing or unreadable icon leaves SDL's default one; the game runs either way.
        private static void ApplyWindowIcon(nint window)
        {
            try
            {
                using Stream resource = typeof(SdlApplication<T>).Assembly.GetManifestResourceStream("Icon.bmp")
                    ?? throw new FileNotFoundException("The Icon.bmp resource is not embedded.");
                using MemoryStream bytes = new();
                resource.CopyTo(bytes);
                WindowIcon.Apply(window, bytes.ToArray());
            }
            catch (Exception failure) when (failure is IOException or InvalidDataException or InvalidOperationException)
            {
                HostLog.WindowIconUnavailable(Log.For(LogCategories.Host), failure.Message);
            }
        }

        private void SetAudioSuspended(bool suspended)
        {
            if (_audio is SdlAudioBackend sdl)
            {
                sdl.Suspended = suspended;
            }
        }
    }
}
