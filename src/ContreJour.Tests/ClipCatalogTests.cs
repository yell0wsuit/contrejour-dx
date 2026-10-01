using ContreJour.Content;

using Mokus2D.Visual;

using Xunit;

namespace ContreJour.Tests
{
    public class ClipCatalogTests
    {
        // McEyeBall is in fakeHero, common and chapter5; McColorFix in lights and chapter4Backgrounds; McTear in
        // chapter5 and level1. The first folder in search order wins.
        [Theory]
        [InlineData("McEyeBall", "fakeHero/McEyeBall", ClipKind.Sprite)]
        [InlineData("McColorFix", "chapter4Backgrounds/McColorFix", ClipKind.Sprite)]
        [InlineData("McTear", "level1/McTear", ClipKind.MovieClip)]
        [InlineData("McSpikesView", "common/McSpikesView", ClipKind.Composite)]
        public void NamesResolveToTheFirstSearchedFolder(string name, string id, ClipKind kind)
        {
            ClipEntry entry = ClipCatalog.Find(name);

            Assert.Equal(id, entry.Id);
            Assert.Equal(kind, entry.Kind);
        }

        // whitePixel lives in menu and McRoseView in menu (and on the skip list); neither folder is searched.
        [Theory]
        [InlineData("whitePixel")]
        [InlineData("McRoseView")]
        [InlineData("NoSuchClip")]
        public void ClipsOutsideTheSearchedFoldersAreNotFound(string name)
        {
            Assert.Null(ClipCatalog.Find(name));
        }

        [Fact]
        public void UnknownNamesBecomePlainNodes()
        {
            Assert.Equal(typeof(Node), ClipCatalog.Create("NoSuchClip").GetType());
        }
    }
}
