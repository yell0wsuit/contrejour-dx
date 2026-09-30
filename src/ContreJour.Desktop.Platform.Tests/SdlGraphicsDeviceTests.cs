using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class SdlGraphicsDeviceTests
    {
        [Theory]
        [InlineData(GraphicsBackendKind.Metal, "Contre Jour | Metal")]
        [InlineData(GraphicsBackendKind.OpenGL, "Contre Jour | OpenGL")]
        [InlineData(GraphicsBackendKind.Software, "Contre Jour | Software")]
        public void TheTitleNamesTheRenderer(GraphicsBackendKind kind, string expected)
        {
            Assert.Equal(expected, SdlGraphicsDevice.TitleFor(kind));
        }
    }
}
