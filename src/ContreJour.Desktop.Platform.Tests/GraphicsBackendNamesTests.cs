using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class GraphicsBackendNamesTests
    {
        [Theory]
        [InlineData("metal", GraphicsBackendKind.Metal)]
        [InlineData("Metal", GraphicsBackendKind.Metal)]
        [InlineData("gl", GraphicsBackendKind.OpenGL)]
        [InlineData("opengl", GraphicsBackendKind.OpenGL)]
        [InlineData("OpenGL", GraphicsBackendKind.OpenGL)]
        [InlineData("software", GraphicsBackendKind.Software)]
        public void CommandLineNamesAreRead(string name, GraphicsBackendKind expected)
        {
            Assert.True(GraphicsBackendNames.TryParse(name, out GraphicsBackendKind kind));
            Assert.Equal(expected, kind);
        }

        [Theory]
        [InlineData("vulkan")]
        [InlineData("")]
        [InlineData(null)]
        public void OtherNamesAreNot(string name)
        {
            Assert.False(GraphicsBackendNames.TryParse(name, out _));
        }
    }
}
