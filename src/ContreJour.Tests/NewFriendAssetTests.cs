using ContreJour.Content;
using Xunit;

namespace ContreJour.Tests
{
    public class NewFriendAssetTests
    {
        [Theory]
        [InlineData("McDragSquareView_7", "newFriend/McDragView_7")]
        [InlineData("McRoundDragSquareView_7", "newFriend/McRoundDragView_7")]
        public void AuthoredDragNamesResolveToWebThemeAliases(string name, string asset)
        {
            Assert.Equal(asset, ClipCatalog.Find(name)?.Id);
        }
    }
}
