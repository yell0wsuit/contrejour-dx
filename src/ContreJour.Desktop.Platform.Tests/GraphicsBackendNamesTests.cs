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
        [InlineData("vulkan", GraphicsBackendKind.Vulkan)]
        [InlineData("Vulkan", GraphicsBackendKind.Vulkan)]
        [InlineData("angle", GraphicsBackendKind.Angle)]
        [InlineData("ANGLE", GraphicsBackendKind.Angle)]
        public void CommandLineNamesAreRead(string name, GraphicsBackendKind expected)
        {
            Assert.True(GraphicsBackendNames.TryParse(name, out GraphicsBackendKind kind));
            Assert.Equal(expected, kind);
        }

        [Theory]
        [InlineData("directx")]
        [InlineData("")]
        [InlineData(null)]
        public void OtherNamesAreNot(string name)
        {
            Assert.False(GraphicsBackendNames.TryParse(name, out _));
        }

        [Fact]
        public void TheExpectedListNamesEveryRendererTheParserAccepts()
        {
            Assert.Equal("metal, vulkan, angle, gl, opengl or software", GraphicsBackendNames.Expected);
            foreach (string name in new[] { "metal", "vulkan", "angle", "gl", "opengl", "software" })
            {
                Assert.True(GraphicsBackendNames.TryParse(name, out _), name);
            }
        }
    }
}
