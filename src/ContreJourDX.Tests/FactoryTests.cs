using System.IO;

using ContreJourDX.Gameplay;

using Xunit;

namespace ContreJourDX.Tests
{
    public class FactoryTests
    {
        [Theory]
        [InlineData("BridgeSnotBodyClip")]
        [InlineData("ColorBallBodyClip")]
        [InlineData("LampBodyClip")]
        [InlineData("MultiSpikesBodyClip")]
        [InlineData("NoSuchClip")]
        public void NamesWithoutAClassGiveNull(string clipType)
        {
            Assert.Null(BodyClipFactory.Create(clipType, null, null, null, null));
        }

        [Fact]
        public void UnknownBackgroundsThrow()
        {
            _ = Assert.Throws<InvalidDataException>(() => BackgroundFactory.Create("ScaleBackground", null, null, null));
        }
    }
}
