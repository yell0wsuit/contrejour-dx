using System;
using System.IO;
using System.Runtime.InteropServices;

using SDL3;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    public class WindowIconTests
    {
        // The file the desktop host embeds as Icon.bmp, copied beside the tests.
        private static byte[] ShippedIcon => File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "ContreJourDXIcon.bmp"));

        [Fact]
        public void ShippedIconLoadsAt256WithAlpha()
        {
            nint surface = WindowIcon.Load(ShippedIcon);
            try
            {
                SDL.Surface info = Marshal.PtrToStructure<SDL.Surface>(surface);
                Assert.Equal(256, info.Width);
                Assert.Equal(256, info.Height);
                Assert.True(SDL.IsPixelFormatAlpha(info.Format));
                Assert.True(SDL.ReadSurfacePixel(surface, 0, 0, out _, out _, out _, out byte corner));
                Assert.True(SDL.ReadSurfacePixel(surface, 128, 128, out _, out _, out _, out byte center));
                Assert.Equal(0, corner);
                Assert.Equal(255, center);
            }
            finally
            {
                SDL.DestroySurface(surface);
            }
        }

        [Fact]
        public void BytesThatAreNotABitmapThrowInvalidData()
        {
            _ = Assert.Throws<InvalidDataException>(() => WindowIcon.Load([1, 2, 3, 4]));
        }
    }
}
