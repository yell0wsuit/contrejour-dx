using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    public class CanvasSizeTests
    {
        [Fact]
        public void UsesTheWindowsPixelSize()
        {
            Assert.Equal((2940, 1846), CanvasSize.Choose(2940, 1846, 1470, 956, 2f));
        }

        [Fact]
        public void FallsBackToTheDisplayWhenTheWindowHasNoArea()
        {
            // A window minimized at launch reports 0x0.
            Assert.Equal((2940, 1912), CanvasSize.Choose(0, 0, 1470, 956, 2f));
        }

        [Fact]
        public void RoundsFractionalDisplayDensity()
        {
            Assert.Equal((2880, 1620), CanvasSize.Choose(0, 0, 1920, 1080, 1.5f));
        }
    }
}
