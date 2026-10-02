using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class SdlGraphicsDeviceTests
    {
        [Theory]
        [InlineData(GraphicsBackendKind.Metal, "Metal")]
        [InlineData(GraphicsBackendKind.OpenGL, "OpenGL")]
        [InlineData(GraphicsBackendKind.Vulkan, "Vulkan")]
        [InlineData(GraphicsBackendKind.Angle, "Angle")]
        [InlineData(GraphicsBackendKind.Software, "Software")]
        public void TheTitleNamesTheRendererAndVersion(GraphicsBackendKind kind, string renderer)
        {
            const string version = "1.2.3";
            Assert.Equal($"Contre Jour v{version} | {renderer}", SdlGraphicsDevice.TitleFor(kind, version));
        }

        [Theory]
        [InlineData("1.0.0-dirty+abcdef0123456789")]
        [InlineData("1.0.0-prerelease+57")]
        public void TheTitlePreservesTheFullBuildVersion(string version)
        {
            Assert.Equal($"Contre Jour v{version} | Metal", SdlGraphicsDevice.TitleFor(GraphicsBackendKind.Metal, version));
        }
    }
}
