using System;

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
    }
}
