using System;

using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class DesktopOptionsTests
    {
        [Fact]
        public void NoArgumentsMeansDefaults()
        {
            DesktopOptions options = DesktopOptions.Parse([]);

            Assert.Null(options.QuitAfterFrames);
        }

        [Fact]
        public void QuitAfterFramesIsRead()
        {
            Assert.Equal(120, DesktopOptions.Parse(["--quit-after-frames", "120"]).QuitAfterFrames);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-5")]
        [InlineData("soon")]
        public void ABadFrameCountIsRejected(string value)
        {
            ArgumentException failure = Assert.Throws<ArgumentException>(() => DesktopOptions.Parse(["--quit-after-frames", value]));

            Assert.Contains(value, failure.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void ASwitchWithoutItsValueIsRejected()
        {
            _ = Assert.Throws<ArgumentException>(() => DesktopOptions.Parse(["--quit-after-frames"]));
        }

        [Fact]
        public void UnknownArgumentsAreIgnored()
        {
            // macOS can add -psn_…; --log-level is LoggingSetup's.
            DesktopOptions options = DesktopOptions.Parse(["-psn_0_12345", "--log-level", "debug", "--something-else"]);

            Assert.Null(options.QuitAfterFrames);
        }

        [Fact]
        public void TheRendererDefaultsToAutomatic()
        {
            Assert.Null(DesktopOptions.Parse([]).Renderer);
            Assert.Null(DesktopOptions.Parse(["--renderer", "auto"]).Renderer);
        }

        [Theory]
        [InlineData("metal", GraphicsBackendKind.Metal)]
        [InlineData("gl", GraphicsBackendKind.OpenGL)]
        [InlineData("opengl", GraphicsBackendKind.OpenGL)]
        [InlineData("software", GraphicsBackendKind.Software)]
        public void ARendererCanBeForced(string name, GraphicsBackendKind expected)
        {
            Assert.Equal(expected, DesktopOptions.Parse(["--renderer", name]).Renderer);
        }

        [Fact]
        public void AnUnknownRendererIsRejectedByName()
        {
            ArgumentException failure = Assert.Throws<ArgumentException>(() => DesktopOptions.Parse(["--renderer", "vulkan"]));

            Assert.Contains("vulkan", failure.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void FailRendererSwitchesBuildTheFaultPlan()
        {
            DesktopOptions options = DesktopOptions.Parse(["--fail-renderer", "metal:after-device", "--fail-renderer", "gl:after-surface"]);

            _ = Assert.Throws<InvalidOperationException>(() => options.Faults.For(GraphicsBackendKind.Metal)("after-device"));
            _ = Assert.Throws<InvalidOperationException>(() => options.Faults.For(GraphicsBackendKind.OpenGL)("after-surface"));
        }

        [Fact]
        public void WithoutFaultsNothingIsInjected()
        {
            DesktopOptions.Parse([]).Faults.For(GraphicsBackendKind.Metal)("after-device");
        }

        [Fact]
        public void DeviceLossesAreCollectedInOrder()
        {
            Assert.Equal([100, 300], DesktopOptions.Parse(["--lose-device", "300", "--lose-device", "100"]).DeviceLosses);
            Assert.Empty(DesktopOptions.Parse([]).DeviceLosses);
        }

        [Fact]
        public void ABadLossFrameIsRejected()
        {
            _ = Assert.Throws<ArgumentException>(() => DesktopOptions.Parse(["--lose-device", "0"]));
        }
    }
}
