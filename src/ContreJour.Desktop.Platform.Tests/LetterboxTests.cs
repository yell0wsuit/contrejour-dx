using System;
using System.Numerics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class LetterboxTests
    {
        private const float Tolerance = 1e-4f;

        [Fact]
        public void SameSizeIsIdentity()
        {
            Letterbox box = new(new Vector2(1600, 1000), new Vector2(1600, 1000), new Vector2(1600, 1000));

            Assert.Equal(1f, box.Scale);
            Assert.Equal(Vector2.Zero, box.Offset);
        }

        [Fact]
        public void WiderTargetCentersWithSideBars()
        {
            Letterbox box = new(new Vector2(1600, 1000), new Vector2(3200, 1000), new Vector2(3200, 1000));

            Assert.Equal(1f, box.Scale);
            Assert.Equal(new Vector2(800, 0), box.Offset);
        }

        [Fact]
        public void TallerTargetCentersWithTopAndBottomBars()
        {
            Letterbox box = new(new Vector2(1600, 1000), new Vector2(800, 1000), new Vector2(800, 1000));

            Assert.Equal(0.5f, box.Scale);
            Assert.Equal(new Vector2(0, 250), box.Offset);
        }

        [Fact]
        public void SmallerTargetOfSameAspectScalesDownWithoutBars()
        {
            Letterbox box = new(new Vector2(1600, 1000), new Vector2(800, 500), new Vector2(800, 500));

            Assert.Equal(0.5f, box.Scale);
            Assert.Equal(Vector2.Zero, box.Offset);
        }

        [Fact]
        public void PixelToLogicalInvertsLogicalToPixel()
        {
            Letterbox box = new(new Vector2(1600, 1000), new Vector2(700, 900), new Vector2(1400, 1800));
            Vector2 logical = new(123.5f, 987.25f);

            Vector2 roundTrip = box.PixelToLogical(box.LogicalToPixel(logical));

            Assert.Equal(logical.X, roundTrip.X, Tolerance);
            Assert.Equal(logical.Y, roundTrip.Y, Tolerance);
        }

        [Fact]
        public void WindowToLogicalScalesPointsByPixelDensity()
        {
            // A Retina full-screen window: 2 pixels per point, canvas = pixel size.
            Letterbox box = new(new Vector2(2940, 1912), new Vector2(1470, 956), new Vector2(2940, 1912));

            Assert.Equal(new Vector2(200, 100), box.WindowToLogical(new Vector2(100, 50)));
            Assert.Equal(new Vector2(100, 50), box.LogicalToWindow(new Vector2(200, 100)));
        }

        [Fact]
        public void WindowToLogicalUsesFractionalDensity()
        {
            // Windows at 150% scale: 1.5 pixels per point.
            Letterbox box = new(new Vector2(1500, 900), new Vector2(1000, 600), new Vector2(1500, 900));

            Vector2 logical = box.WindowToLogical(new Vector2(100, 100));

            Assert.Equal(150f, logical.X, Tolerance);
            Assert.Equal(150f, logical.Y, Tolerance);
        }

        [Fact]
        public void WindowToLogicalAccountsForBars()
        {
            // Canvas 1600x1000 in a 1600x1600 pixel window at 1x: bars of 300 above and below.
            Letterbox box = new(new Vector2(1600, 1000), new Vector2(1600, 1600), new Vector2(1600, 1600));

            Assert.Equal(new Vector2(0, 0), box.WindowToLogical(new Vector2(0, 300)));
            Assert.Equal(new Vector2(0, -300), box.WindowToLogical(new Vector2(0, 0)));
        }

        [Theory]
        [InlineData(0, 1000, 800, 500, 800, 500)]
        [InlineData(1600, 1000, 0, 500, 800, 500)]
        [InlineData(1600, 1000, 800, 500, 800, 0)]
        [InlineData(1600, 1000, 800, -1, 800, 500)]
        public void ConstructorRejectsEmptySizes(float lw, float lh, float ww, float wh, float pw, float ph)
        {
            _ = Assert.Throws<ArgumentOutOfRangeException>(
                () => new Letterbox(new Vector2(lw, lh), new Vector2(ww, wh), new Vector2(pw, ph)));
        }
    }
}
