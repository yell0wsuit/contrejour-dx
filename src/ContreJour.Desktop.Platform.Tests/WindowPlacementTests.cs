using SDL3;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class WindowPlacementTests
    {
        [Fact]
        public void NormalResizesUpdateTheSizeToReopen()
        {
            WindowPlacement state = new(1000, 700, false);
            Assert.True(state.Refresh(0, 1100, 750));
            Assert.Equal((1100, 750), (state.Width, state.Height));
            Assert.False(state.Refresh(0, 1100, 750));
        }

        [Fact]
        public void MaximizingAndRestoringKeepTheNormalDimensions()
        {
            WindowPlacement state = new(1000, 700, false);
            Assert.True(state.Refresh(SDL.WindowFlags.Maximized, 1470, 820));
            Assert.True(state.Maximized);
            Assert.Equal((1000, 700), (state.Width, state.Height));
            Assert.True(state.Refresh(0, 1000, 700));
            Assert.False(state.Maximized);
        }

        [Theory]
        [InlineData(SDL.WindowFlags.Fullscreen)]
        [InlineData(SDL.WindowFlags.Fullscreen | SDL.WindowFlags.Maximized)]
        [InlineData(SDL.WindowFlags.Minimized)]
        [InlineData(SDL.WindowFlags.Minimized | SDL.WindowFlags.Maximized)]
        public void FullscreenAndMinimizedSnapshotsPreserveWindowedPlacement(SDL.WindowFlags flags)
        {
            WindowPlacement state = new(1000, 700, true);
            Assert.False(state.Refresh(flags, 1470, 956));
            Assert.True(state.Maximized);
            Assert.Equal((1000, 700), (state.Width, state.Height));
        }

        [Fact]
        public void HiddenWindowDoesNotClearSavedMaximization()
        {
            WindowPlacement state = new(1000, 700, true);
            Assert.False(state.Refresh(0, 1000, 700));
            Assert.True(state.Maximized);
        }

        [Fact]
        public void FullscreenRoundTripPreservesMaximization()
        {
            WindowPlacement state = new(1000, 700, false);
            _ = state.Refresh(SDL.WindowFlags.Maximized, 1470, 820);
            _ = state.Refresh(SDL.WindowFlags.Fullscreen, 1470, 956);
            _ = state.Refresh(SDL.WindowFlags.Maximized, 1470, 820);
            Assert.True(state.Maximized);
            Assert.Equal((1000, 700), (state.Width, state.Height));
        }

        [Fact]
        public void GeometryDerivedMaximizationDoesNotChangeThePlayersPreference()
        {
            WindowPlacement state = new(1000, 700, false);
            _ = state.Refresh(SDL.WindowFlags.Fullscreen, 1470, 956);
            Assert.False(state.Refresh(SDL.WindowFlags.Maximized, 1000, 700));
            Assert.False(state.Maximized);
        }

        [Fact]
        public void FrameFittingDoesNotTurnGeometryDerivedZoomIntoPlayerMaximization()
        {
            WindowPlacement state = new(1470, 930, false);
            state.SetNormalSize(1470, 928);
            Assert.False(state.Refresh(SDL.WindowFlags.Maximized, 1470, 928));
            Assert.False(state.Maximized);
            Assert.Equal((1470, 928), (state.Width, state.Height));
        }

        [Fact]
        public void ZeroSizedSnapshotsAreIgnored()
        {
            WindowPlacement state = new(1000, 700, false);
            Assert.False(state.Refresh(0, 0, 0));
            Assert.Equal((1000, 700), (state.Width, state.Height));
        }

        [Theory]
        [InlineData(0, 1000, 0, 800)]
        [InlineData(-1, 1000, 0, 800)]
        [InlineData(2000, 1000, 30, 970)]
        [InlineData(700, 1000, 30, 700)]
        [InlineData(9000, 10000, 0, 4096)]
        [InlineData(1, 1000, 0, 320)]
        [InlineData(1000, 200, 30, 170)]
        public void SavedSizesFitTheUsableDisplayAndFrame(int requested, int usable, int decoration, int expected)
        {
            Assert.Equal(expected, WindowPlacement.ClampSide(requested, usable, decoration));
        }
    }
}
