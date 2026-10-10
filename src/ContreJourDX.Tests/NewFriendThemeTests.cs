using System.Reflection;

using ContreJourDX.Gameplay;

using Mokus2D.Graphics;

using Xunit;

namespace ContreJourDX.Tests
{
    public class NewFriendThemeTests
    {
        [Fact]
        public void GameplayButtonsUsePurpleOnlyForNewFriend()
        {
            Assert.Equal(new Color(128, 108, 141), ContreJourDXGame.GetButtonsColor(Constants.NewFriendChapter));
            Assert.Equal(ContreJourDXConstants.GreyColor, ContreJourDXGame.GetButtonsColor(0));
            Assert.Equal(ContreJourDXConstants.GreyColor, ContreJourDXGame.GetButtonsColor(4));
        }

        [Theory]
        [InlineData("FontColors")]
        [InlineData("BackColors")]
        public void NewFriendMenuPaletteUsesPurpleAndKeepsEarlierChapters(string paletteName)
        {
            FieldInfo field = typeof(MainMenu).GetField(paletteName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(field);
            Color[] palette = Assert.IsType<Color[]>(field.GetValue(null));
            Assert.Equal(Constants.ChaptersCount, palette.Length);
            Assert.Equal(ContreJourDXConstants.NewFriendColor, palette[5]);
            Assert.Equal(new Color(128, 108, 141), palette[5]);
            Assert.Equal(ContreJourDXConstants.GreyColor, palette[0]);
            Assert.Equal(ContreJourDXConstants.GreyColor, palette[4]);
        }
    }
}
