using ContreJourDX.Desktop.Platform.Graphics;

using SkiaSharp;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    public class DrawCheckTests
    {
        private const int Width = 64;

        private const int Height = 48;

        private static SKSurface Raster()
        {
            return SKSurface.Create(new SKImageInfo(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul));
        }

        private static SKColor SampleOf(SKSurface surface)
        {
            using SKImage image = surface.Snapshot();
            using SKBitmap bitmap = SKBitmap.FromImage(image);
            SKPointI at = DrawCheck.Sample(Width, Height);
            return bitmap.GetPixel(at.X, at.Y);
        }

        [Fact]
        public void AShadedDrawLeavesSomethingOtherThanTheBackground()
        {
            using SKSurface surface = Raster();

            DrawCheck.Draw(surface.Canvas, Width, Height);

            Assert.True(DrawCheck.Drew(SampleOf(surface)));
        }

        [Fact]
        public void AClearAloneIsNotAccepted()
        {
            using SKSurface surface = Raster();

            surface.Canvas.Clear(DrawCheck.Background);

            Assert.False(DrawCheck.Drew(SampleOf(surface)));
        }

        [Fact]
        public void AReadbackThatWasNeverWrittenIsNotAccepted()
        {
            // A driver that reports a successful readback without writing leaves the buffer zeroed.
            Assert.False(DrawCheck.Drew(new SKColor(0, 0, 0, 0)));
        }

        [Fact]
        public void TheSampleSitsInsideTheTarget()
        {
            SKPointI at = DrawCheck.Sample(Width, Height);

            Assert.InRange(at.X, 0, Width - 1);
            Assert.InRange(at.Y, 0, Height - 1);
        }

        [Fact]
        public void TheDrawCoversTheWholeTarget()
        {
            using SKSurface surface = Raster();

            DrawCheck.Draw(surface.Canvas, Width, Height);

            using SKImage image = surface.Snapshot();
            using SKBitmap bitmap = SKBitmap.FromImage(image);
            Assert.NotEqual(DrawCheck.Background, bitmap.GetPixel(0, 0));
            Assert.NotEqual(DrawCheck.Background, bitmap.GetPixel(Width - 1, Height - 1));
        }
    }
}
