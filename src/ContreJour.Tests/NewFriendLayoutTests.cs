using System.Numerics;

using ContreJour.Gameplay;

using Xunit;

namespace ContreJour.Tests
{
    public class NewFriendLayoutTests
    {
        [Theory]
        [InlineData(960f, 540f)]
        [InlineData(960f, 720f)]
        [InlineData(1920f, 540f)]
        public void AuthoredLevelFitsBothViewportDimensions(float width, float height)
        {
            Vector2 level = new(960f, 640f);
            float scale = ContreJourGame.GetLevelScale(level, new Vector2(width, height), true);
            Assert.True(level.X * scale <= width);
            Assert.True(level.Y * scale <= height);
            Assert.Equal(width / level.X, ContreJourGame.GetLevelScale(level, new Vector2(width, height), false));
        }
    }
}
