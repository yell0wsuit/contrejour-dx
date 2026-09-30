using System.Collections.Generic;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class FullScreenStateTests
    {
        // A window that applies requests unless told to refuse, ignore or not finish them.
        private sealed class FakeWindow
        {
            public bool FullScreen { get; set; }

            public bool Refuses { get; set; }

            public bool Ignores { get; set; }

            public bool Settles { get; set; } = true;

            public List<bool> Requests { get; } = [];

            public FullScreenState CreateState()
            {
                return new FullScreenState(() => FullScreen, Request, () => Settles) { IsFullScreen = FullScreen };
            }

            private bool Request(bool fullScreen)
            {
                Requests.Add(fullScreen);
                if (Refuses)
                {
                    return false;
                }
                if (!Ignores && Settles)
                {
                    FullScreen = fullScreen;
                }
                return true;
            }
        }

        [Fact]
        public void ApplyAsksTheWindowForTheRequestedState()
        {
            FakeWindow window = new();
            FullScreenState state = window.CreateState();

            state.IsFullScreen = true;
            state.Apply();

            Assert.Equal([true], window.Requests);
            Assert.True(state.IsFullScreen);
        }

        [Fact]
        public void ApplyDoesNothingWhenTheWindowAlreadyMatches()
        {
            FakeWindow window = new() { FullScreen = true };
            FullScreenState state = window.CreateState();

            state.Apply();

            Assert.Empty(window.Requests);
        }

        [Fact]
        public void RefusedRequestFallsBackToTheWindowsState()
        {
            FakeWindow window = new() { Refuses = true };
            FullScreenState state = window.CreateState();

            state.IsFullScreen = true;
            state.Apply();

            Assert.False(state.IsFullScreen);
        }

        [Fact]
        public void IgnoredRequestReadsTheWindowBackOnceSettled()
        {
            FakeWindow window = new() { Ignores = true };
            FullScreenState state = window.CreateState();

            state.IsFullScreen = true;
            state.Apply();

            Assert.False(state.IsFullScreen);
        }

        [Fact]
        public void NextToggleAfterAnIgnoredRequestAsksAgain()
        {
            FakeWindow window = new() { Ignores = true };
            FullScreenState state = window.CreateState();
            state.IsFullScreen = true;
            state.Apply();

            state.IsFullScreen = !state.IsFullScreen;
            state.Apply();

            Assert.Equal([true, true], window.Requests);
        }

        [Fact]
        public void UnsettledRequestKeepsTheRequestUntilTheWindowReports()
        {
            FakeWindow window = new() { Settles = false };
            FullScreenState state = window.CreateState();

            state.IsFullScreen = true;
            state.Apply();
            bool whileInFlight = state.IsFullScreen;
            state.OnWindowChanged(false);

            Assert.True(whileInFlight);
            Assert.False(state.IsFullScreen);
        }

        [Fact]
        public void WindowChangesFromOutsideTheGameAreFollowed()
        {
            FakeWindow window = new() { FullScreen = true };
            FullScreenState state = window.CreateState();

            // The green button or SDL's own menu item.
            state.OnWindowChanged(false);

            Assert.False(state.IsFullScreen);
        }
    }
}
