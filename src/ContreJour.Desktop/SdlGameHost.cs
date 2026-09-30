using System;
using System.Numerics;

using ContreJour.Desktop.Platform;

using Mokus2D.Game;
using Mokus2D.Util.Data;

using SDL3;

namespace ContreJour.Desktop
{
    // The SDL window as the engine sees it. The back buffer is a fixed logical canvas, the window's
    // pixel size when it first went full screen: the game lays itself out once, at startup, so
    // later window sizes are letterboxed rather than handed to it.
    internal sealed class SdlGameHost : IGameHost
    {
        // Share of the display's usable area the window takes when it leaves full screen.
        private const float WindowedFraction = 0.8f;

        private readonly nint _window;

        public SdlGameHost(nint window)
        {
            _window = window;
            // Sized and placed before going full screen, so every way out of full screen (F11, the
            // green button, ⌃⌘F from SDL's own menu) comes back to this window.
            PlaceWindowed();
            _ = SDL.SetWindowFullscreen(_window, true);
            Check(SDL.ShowWindow(_window));
            _ = SDL.SyncWindow(_window);
            // macOS animates into a full-screen Space and SyncWindow can return before it ends. Wait
            // (up to two seconds) for the flag before reading the size the canvas is fixed at;
            // PumpEvents only queues events, so none are lost before the loop starts.
            for (int i = 0; i < 200 && (SDL.GetWindowFlags(_window) & SDL.WindowFlags.Fullscreen) == 0; i++)
            {
                SDL.PumpEvents();
                SDL.Delay(10);
            }
            Check(SDL.GetWindowSizeInPixels(_window, out int width, out int height));
            BackBufferSize = new Point(width, height);
            PreferredBackBufferSize = BackBufferSize;
            IsFullScreen = (SDL.GetWindowFlags(_window) & SDL.WindowFlags.Fullscreen) != 0;
            IsActive = (SDL.GetWindowFlags(_window) & SDL.WindowFlags.InputFocus) != 0;
            Letterbox = new Letterbox(new Vector2(width, height), WindowPoints(), new Vector2(width, height));
            RefreshLetterbox();
        }

        public event Action ClientSizeChanged;

        public event Action<bool> ActiveChanged;

        public Point BackBufferSize { get; }

        public Vector2 WindowSize => new(BackBufferSize.X, BackBufferSize.Y);

        public Rectangle ClientBounds
        {
            get
            {
                Check(SDL.GetWindowPosition(_window, out int x, out int y));
                Check(SDL.GetWindowSize(_window, out int width, out int height));
                return new Rectangle(x, y, width, height);
            }
        }

        public bool IsActive { get; private set; }

        // The requested state until ApplyGraphicsChanges, then the window's; SDL's own menu and the
        // green button change it without asking the host.
        public bool IsFullScreen { get; set; }

        public Point PreferredBackBufferSize { get; set; }

        // The loop always steps variable time; these are kept for the engine to read back.
        public bool IsFixedTimeStep { get; set; }

        public TimeSpan TargetElapsedTime { get; set; }

        public bool IsMouseVisible
        {
            get;
            set
            {
                field = value;
                _ = value ? SDL.ShowCursor() : SDL.HideCursor();
            }
        } = true;

        public bool QuitRequested { get; private set; }

        // Always built from positive sizes: a minimized window keeps the last one.
        public Letterbox Letterbox { get; private set; }

        public void ApplyGraphicsChanges()
        {
            if (PreferredBackBufferSize != BackBufferSize)
            {
                throw new NotSupportedException("The desktop back buffer is fixed at startup; windows are letterboxed instead.");
            }
            bool windowFullScreen = (SDL.GetWindowFlags(_window) & SDL.WindowFlags.Fullscreen) != 0;
            if (IsFullScreen != windowFullScreen)
            {
                _ = SDL.SetWindowFullscreen(_window, IsFullScreen);
                _ = SDL.SyncWindow(_window);
            }
        }

        public void Quit()
        {
            QuitRequested = true;
        }

        public void WarpMouse(Vector2 windowPoint)
        {
            SDL.WarpMouseInWindow(_window, windowPoint.X, windowPoint.Y);
        }

        public void HandleEvent(in SDL.Event e)
        {
            // Deliberately not a switch: the populate-switch fixer rewrites one over this enum into
            // every one of its members.
            SDL.EventType type = (SDL.EventType)e.Type;
            if (type is SDL.EventType.Quit or SDL.EventType.WindowCloseRequested)
            {
                QuitRequested = true;
            }
            else if (type == SDL.EventType.WindowFocusGained)
            {
                SetActive(true);
            }
            else if (type is SDL.EventType.WindowFocusLost or SDL.EventType.WindowMinimized)
            {
                SetActive(false);
            }
            else if (type is SDL.EventType.WindowEnterFullscreen or SDL.EventType.WindowLeaveFullscreen)
            {
                IsFullScreen = type == SDL.EventType.WindowEnterFullscreen;
            }
            else if (type is SDL.EventType.WindowResized or SDL.EventType.WindowPixelSizeChanged or SDL.EventType.WindowDisplayScaleChanged)
            {
                RefreshLetterbox();
                ClientSizeChanged?.Invoke();
            }
            else if (SdlInputState.IsFullScreenShortcut(e))
            {
                IsFullScreen = !IsFullScreen;
                ApplyGraphicsChanges();
            }
        }

        private void SetActive(bool active)
        {
            if (active == IsActive)
            {
                return;
            }
            IsActive = active;
            ActiveChanged?.Invoke(active);
        }

        private void RefreshLetterbox()
        {
            Check(SDL.GetWindowSizeInPixels(_window, out int pixelWidth, out int pixelHeight));
            Vector2 points = WindowPoints();
            if (pixelWidth <= 0 || pixelHeight <= 0 || points.X <= 0 || points.Y <= 0)
            {
                return;
            }
            Letterbox = new Letterbox(new Vector2(BackBufferSize.X, BackBufferSize.Y), points, new Vector2(pixelWidth, pixelHeight));
        }

        private Vector2 WindowPoints()
        {
            Check(SDL.GetWindowSize(_window, out int width, out int height));
            return new Vector2(width, height);
        }

        private void PlaceWindowed()
        {
            uint display = SDL.GetDisplayForWindow(_window);
            if (!SDL.GetDisplayUsableBounds(display, out SDL.Rect bounds))
            {
                return;
            }
            _ = SDL.SetWindowSize(_window, (int)(bounds.W * WindowedFraction), (int)(bounds.H * WindowedFraction));
            int centered = (int)SDL.WindowPosCenteredDisplay((int)display);
            _ = SDL.SetWindowPosition(_window, centered, centered);
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
