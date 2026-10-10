using Mokus2D.Util.Data;

using Xunit;

namespace ContreJourDX.Browser.Platform.Tests
{
    public class BrowserCanvasTests
    {
        [Fact]
        public void LandscapeScreenAtTwoX()
        {
            Assert.Equal(new Point(2940, 1912), BrowserCanvas.LogicalSize(1470, 956, 2));
        }

        [Fact]
        public void PortraitScreenIsTurnedLandscape()
        {
            Assert.Equal(new Point(1688, 780), BrowserCanvas.LogicalSize(390, 844, 2));
        }

        [Fact]
        public void PixelRatioIsCappedAtTwo()
        {
            Assert.Equal(new Point(1688, 780), BrowserCanvas.LogicalSize(390, 844, 3));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(double.NaN)]
        public void UnusableRatioCountsAsOne(double ratio)
        {
            Assert.Equal(new Point(1280, 720), BrowserCanvas.LogicalSize(1280, 720, ratio));
        }

        [Fact]
        public void FractionalRatioRounds()
        {
            Assert.Equal(new Point(1920, 1080), BrowserCanvas.LogicalSize(1536, 864, 1.25));
        }

        [Fact]
        public void EmptyScreenStillGivesOnePixel()
        {
            Assert.Equal(new Point(1, 1), BrowserCanvas.LogicalSize(0, 0, 1));
        }
    }
}
