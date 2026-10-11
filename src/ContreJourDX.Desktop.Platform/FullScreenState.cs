using System;

namespace ContreJourDX.Desktop.Platform
{
    // The window's full-screen state as the engine sees it. Setting IsFullScreen records a request;
    // Apply asks the window for it and then takes the window's word, so a request the window
    // manager refuses or ignores cannot leave the two disagreeing (a later toggle would otherwise
    // only flip the value back and do nothing on screen).
    public sealed class FullScreenState
    {
        private readonly Func<bool> _windowIsFullScreen;

        private readonly Func<bool, bool> _request;

        private readonly Func<bool> _settle;

        // windowIsFullScreen: the window's current state; request: asks for a state, false when
        // refused outright; settle: waits for pending window changes, false when they are still
        // in flight.
        public FullScreenState(Func<bool> windowIsFullScreen, Func<bool, bool> request, Func<bool> settle)
        {
            ArgumentNullException.ThrowIfNull(windowIsFullScreen);
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(settle);
            _windowIsFullScreen = windowIsFullScreen;
            _request = request;
            _settle = settle;
        }

        public bool IsFullScreen { get; set; }

        public void Apply()
        {
            bool window = _windowIsFullScreen();
            if (IsFullScreen == window)
            {
                return;
            }
            if (!_request(IsFullScreen))
            {
                IsFullScreen = window;
            }
            else if (_settle())
            {
                IsFullScreen = _windowIsFullScreen();
            }
            // Otherwise the change is still in flight, and the window's enter/leave event settles it.
        }

        // The window entered or left full screen: on request, or through the system's own controls.
        public void OnWindowChanged(bool fullScreen)
        {
            IsFullScreen = fullScreen;
        }
    }
}
