using System.IO;

using ContreJour.Gameplay;

using Xunit;

namespace ContreJour.Tests
{
    public class FactoryTests
    {
        [Theory]
        [InlineData("BaloonBodyClip")]
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
