using System;

using ContreJourDX.Desktop;
using ContreJourDX.Saving;

using SDL3;

using Xunit;

namespace ContreJourDX.Tests
{
    // Native SDL's dummy video driver exercises the host without showing a window or changing a
    // desktop Space. Window-manager zoom/animation behavior still needs a real desktop check.
    public sealed class SdlWindowPreferencesTests : IDisposable
    {
        private readonly string _previousDriver = SDL.GetHint("SDL_VIDEO_DRIVER");
        private nint _window;

        public SdlWindowPreferencesTests()
        {
            Assert.True(SDL.SetHint("SDL_VIDEO_DRIVER", "dummy"));
            Assert.True(SDL.InitSubSystem(SDL.InitFlags.Video), SDL.GetError());
            Preferences.Settings.Clear();
            Preferences.Settings.SetInt("PREFS_WINDOW_WIDTH", 640);
            Preferences.Settings.SetInt("PREFS_WINDOW_HEIGHT", 480);
            Preferences.Settings.SetBool("PREFS_WINDOW_FULLSCREEN", false);
            _window = CreateWindow();
        }

        private static nint CreateWindow()
        {
            nint window = SDL.CreateWindow("Window preference test", 100, 100, SDL.WindowFlags.Hidden | SDL.WindowFlags.Resizable);
            Assert.NotEqual(0, window);
            return window;
        }

        [Fact]
        public void WindowedStartupRestoresSizeAndKeepsADisplaySizedCanvas()
        {
            SdlGameHost host = new(_window);
            Assert.False(host.IsFullScreen);
            Assert.True(SDL.GetWindowSize(_window, out int width, out int height));
            Assert.Equal((640, 480), (width, height));
            SDL.DisplayMode display = SDL.GetDesktopDisplayMode(SDL.GetDisplayForWindow(_window)).Value;
            Assert.Equal((int)MathF.Round(display.W * display.PixelDensity), host.BackBufferSize.X);
            Assert.Equal((int)MathF.Round(display.H * display.PixelDensity), host.BackBufferSize.Y);
        }

        [Fact]
        public void ResizeEventsSaveNormalDimensionsAndReplacementWindowsRestoreThem()
        {
            SdlGameHost host = new(_window);
            Assert.True(SDL.SetWindowSize(_window, 700, 500));
            SDL.Event resized = default;
            resized.Window.Type = SDL.EventType.WindowResized;
            resized.Window.WindowID = SDL.GetWindowID(_window);
            host.HandleEvent(resized);
            Assert.Equal(700, Preferences.Settings.GetInt("PREFS_WINDOW_WIDTH"));
            Assert.Equal(500, Preferences.Settings.GetInt("PREFS_WINDOW_HEIGHT"));
            host.DetachWindow();
            SDL.DestroyWindow(_window);
            _window = CreateWindow();
            host.AttachWindow(_window);
            Assert.False(host.IsFullScreen);
            Assert.True(SDL.GetWindowSize(_window, out int width, out int height));
            Assert.Equal((700, 500), (width, height));
        }

        [Fact]
        public void FullscreenRequestsSaveActualModeWithoutReplacingNormalDimensions()
        {
            SdlGameHost host = new(_window) { IsFullScreen = true };
            host.ApplyGraphicsChanges();
            Assert.True(host.IsFullScreen);
            Assert.True(Preferences.Settings.GetBool("PREFS_WINDOW_FULLSCREEN"));
            Assert.Equal(640, Preferences.Settings.GetInt("PREFS_WINDOW_WIDTH"));
            Assert.Equal(480, Preferences.Settings.GetInt("PREFS_WINDOW_HEIGHT"));
            host.IsFullScreen = false;
            host.ApplyGraphicsChanges();
            Assert.False(host.IsFullScreen);
            Assert.False(Preferences.Settings.GetBool("PREFS_WINDOW_FULLSCREEN", true));
            Assert.True(SDL.GetWindowSize(_window, out int width, out int height));
            Assert.Equal((640, 480), (width, height));
        }

        public void Dispose()
        {
            SDL.DestroyWindow(_window);
            SDL.QuitSubSystem(SDL.InitFlags.Video);
            _ = SDL.SetHint("SDL_VIDEO_DRIVER", _previousDriver);
            Preferences.Settings.Clear();
        }
    }
}
