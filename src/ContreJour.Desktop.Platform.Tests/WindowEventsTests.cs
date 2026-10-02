using SDL3;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class WindowEventsTests
    {
        private static SDL.Event WindowEvent(SDL.EventType type, uint windowId)
        {
            SDL.Event e = default;
            e.Window.Type = type;
            e.Window.WindowID = windowId;
            return e;
        }

        [Theory]
        [InlineData(SDL.EventType.WindowFocusLost)]
        [InlineData(SDL.EventType.WindowFocusGained)]
        [InlineData(SDL.EventType.WindowMinimized)]
        [InlineData(SDL.EventType.WindowResized)]
        [InlineData(SDL.EventType.WindowPixelSizeChanged)]
        [InlineData(SDL.EventType.WindowDisplayScaleChanged)]
        [InlineData(SDL.EventType.WindowEnterFullscreen)]
        [InlineData(SDL.EventType.WindowLeaveFullscreen)]
        [InlineData(SDL.EventType.WindowCloseRequested)]
        public void AWindowEventFromAnotherWindowIsForeign(SDL.EventType type)
        {
            // A window a lost device took with it can still have events queued.
            Assert.True(WindowEvents.IsForOtherWindow(WindowEvent(type, 7), 9));
            Assert.False(WindowEvents.IsForOtherWindow(WindowEvent(type, 9), 9));
        }

        [Fact]
        public void EventsThatAreNotWindowEventsNeverAre()
        {
            SDL.Event quit = default;
            quit.Type = (uint)SDL.EventType.Quit;
            SDL.Event key = default;
            key.Key.Type = SDL.EventType.KeyDown;
            key.Key.WindowID = 7;

            Assert.False(WindowEvents.IsForOtherWindow(quit, 9));
            Assert.False(WindowEvents.IsForOtherWindow(key, 9));
        }
    }
}
