using System;
using System.Numerics;

using ContreJourDX.Desktop.Platform;

using Mokus2D.Game;
using Mokus2D.Util.Data;

using SDL3;

namespace ContreJourDX.Desktop
{
    // The SDL window as the engine sees it. The back buffer is a fixed logical canvas, sized from
    // the fullscreen surface or display at startup: the game lays itself out once, at startup, so
    // later window sizes are letterboxed rather than handed to it.
    internal sealed class SdlGameHost : IGameHost
    {
        private WindowPlacement _placement;

        private nint _window;

        private uint _windowId;

        private readonly FullScreenState _fullScreen;

        public SdlGameHost(nint window)
        {
            _window = window;
            _windowId = SDL.GetWindowID(window);
            (_placement, bool fullScreen) = WindowPreferences.Read();
            Reveal(fullScreen);
            Check(SDL.GetWindowSizeInPixels(_window, out int pixelWidth, out int pixelHeight));
            SDL.DisplayMode display = DesktopMode();
            (int width, int height) = CanvasSize.Choose(WindowIsFullScreen() ? pixelWidth : 0, WindowIsFullScreen() ? pixelHeight : 0, display.W, display.H, display.PixelDensity);
            BackBufferSize = new Point(width, height);
            PreferredBackBufferSize = BackBufferSize;
            _fullScreen = new FullScreenState(WindowIsFullScreen, fullScreen => SDL.SetWindowFullscreen(_window, fullScreen), () => SDL.SyncWindow(_window))
            {
                IsFullScreen = WindowIsFullScreen()
            };
            IsActive = HasFocus();
            // Identity until the window has an area; RefreshLetterbox keeps it while it has none.
            Vector2 canvas = new(width, height);
            Letterbox = new Letterbox(canvas, canvas, canvas);
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
        public bool IsFullScreen
        {
            get => _fullScreen.IsFullScreen;
            set => _fullScreen.IsFullScreen = value;
        }

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
            _fullScreen.Apply();
            FitWindowedSizeToFrame();
            RefreshWindowPreferences();
        }

        public void Quit()
        {
            QuitRequested = true;
        }

        public void WarpMouse(Vector2 windowPoint)
        {
            SDL.WarpMouseInWindow(_window, windowPoint.X, windowPoint.Y);
        }

        // Before the window's device is released: the game is deactivated as if the window had lost
        // focus, so the level pauses and nothing held carries over.
        public void DetachWindow()
        {
            RefreshWindowPreferences();
            SetActive(false);
            _window = 0;
            _windowId = 0;
        }

        // A replacement device's window, shown the way the old one was. The canvas is unchanged: the
        // game laid itself out once, and the new window is letterboxed like any other size.
        public void AttachWindow(nint window)
        {
            _window = window;
            _windowId = SDL.GetWindowID(window);
            Reveal(_fullScreen.IsFullScreen);
            _fullScreen.OnWindowChanged(WindowIsFullScreen());
            RefreshLetterbox();
            ClientSizeChanged?.Invoke();
            SetActive(HasFocus());
        }

        public void HandleEvent(in SDL.Event e)
        {
            // Deliberately not a switch: the populate-switch fixer rewrites one over this enum into
            // every one of its members.
            SDL.EventType type = (SDL.EventType)e.Type;
            if (WindowEvents.IsForOtherWindow(e, _windowId))
            {
                return;
            }
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
                _fullScreen.OnWindowChanged(type == SDL.EventType.WindowEnterFullscreen);
                FitWindowedSizeToFrame();
                RefreshWindowPreferences();
                RefreshLetterbox();
                ClientSizeChanged?.Invoke();
            }
            else if (type is SDL.EventType.WindowResized or SDL.EventType.WindowPixelSizeChanged or SDL.EventType.WindowDisplayScaleChanged
                or SDL.EventType.WindowMaximized or SDL.EventType.WindowRestored)
            {
                RefreshWindowPreferences();
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

        private bool WindowIsFullScreen()
        {
            return (SDL.GetWindowFlags(_window) & SDL.WindowFlags.Fullscreen) != 0;
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

        private SDL.DisplayMode DesktopMode()
        {
            return SDL.GetDesktopDisplayMode(SDL.GetDisplayForWindow(_window))
                ?? throw new InvalidOperationException($"SDL could not read the display mode: {SDL.GetError()}");
        }

        private (int Width, int Height) FitToDisplay(int width, int height)
        {
            Check(SDL.GetDisplayUsableBounds(SDL.GetDisplayForWindow(_window), out SDL.Rect bounds));
            _ = SDL.GetWindowBordersSize(_window, out int top, out int left, out int bottom, out int right);
            return (WindowPlacement.ClampSide(width, bounds.W, left + right),
                WindowPlacement.ClampSide(height, bounds.H, top + bottom));
        }

        private void Center()
        {
            int centered = (int)SDL.WindowPosCenteredDisplay((int)SDL.GetDisplayForWindow(_window));
            _ = SDL.SetWindowPosition(_window, centered, centered);
        }

        // Some backends can measure the frame only after the window is shown or leaves fullscreen.
        private void FitWindowedSizeToFrame()
        {
            if ((SDL.GetWindowFlags(_window) & (SDL.WindowFlags.Fullscreen | SDL.WindowFlags.Maximized | SDL.WindowFlags.Minimized)) != 0)
            {
                return;
            }
            (int width, int height) = FitToDisplay(_placement.Width, _placement.Height);
            bool shrunk = width != _placement.Width || height != _placement.Height;
            _placement.SetNormalSize(width, height);
            Check(SDL.GetWindowSize(_window, out int currentWidth, out int currentHeight));
            if (width != currentWidth || height != currentHeight)
            {
                _ = SDL.SetWindowSize(_window, width, height);
                _ = SDL.SyncWindow(_window);
                if (shrunk)
                {
                    Center();
                }
            }
        }

        private void RefreshWindowPreferences()
        {
            Check(SDL.GetWindowSize(_window, out int width, out int height));
            SDL.WindowFlags flags = SDL.GetWindowFlags(_window);
            _ = _placement.Refresh(flags, width, height);
            WindowPreferences.Save(_placement, (flags & SDL.WindowFlags.Fullscreen) != 0);
        }

        private static void Check(bool success)
        {
            if (!success)
            {
                throw new InvalidOperationException(SDL.GetError());
            }
        }

        // Size the normal window while hidden, before fullscreen or maximization can obscure it.
        private void Reveal(bool fullScreen)
        {
            (int width, int height) = FitToDisplay(_placement.Width, _placement.Height);
            _placement = new WindowPlacement(width, height, _placement.Maximized);
            _ = SDL.SetWindowMinimumSize(_window, 320, 320);
            _ = SDL.SetWindowSize(_window, width, height);
            Center();
            if (fullScreen)
            {
                _ = SDL.SetWindowFullscreen(_window, true);
            }
            Check(SDL.ShowWindow(_window));
            _ = SDL.SyncWindow(_window);
            // macOS animates into a full-screen Space and SyncWindow can return before it ends. Wait
            // (up to two seconds) for the flag; PumpEvents only queues events, so none are lost.
            for (int i = 0; fullScreen && i < 200 && !WindowIsFullScreen(); i++)
            {
                SDL.PumpEvents();
                SDL.Delay(10);
            }
            FitWindowedSizeToFrame();
            // Cocoa's maximize request is a zoom toggle; do not toggle an already zoomed window.
            if (_placement.Maximized && !WindowIsFullScreen() && (SDL.GetWindowFlags(_window) & SDL.WindowFlags.Maximized) == 0)
            {
                _ = SDL.MaximizeWindow(_window);
                _ = SDL.SyncWindow(_window);
            }
            RefreshWindowPreferences();
        }

        private bool HasFocus()
        {
            return (SDL.GetWindowFlags(_window) & SDL.WindowFlags.InputFocus) != 0;
        }
    }
}
