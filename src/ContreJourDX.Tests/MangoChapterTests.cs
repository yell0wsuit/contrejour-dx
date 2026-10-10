using System.Linq;
using System.Reflection;

using ContreJourDX.Content;
using ContreJourDX.Gameplay;
using ContreJourDX.Saving;

using Mokus2D.Graphics;

using Xunit;

namespace ContreJourDX.Tests
{
    public class MangoChapterTests
    {
        private static readonly int[] MangoLevels = [188, 171, 170, 172, 190, 196, 191, 176, 186, 174,
            194, 178, 195, 182, 189, 197, 198, 183, 179, 199];

        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 5)]
        [InlineData(3, 10)]
        [InlineData(4, 15)]
        [InlineData(5, 20)]
        [InlineData(6, 20)]
        [InlineData(7, 20)]
        public void MangoCannotSkipOrAdvancePastAvailableRows(int unlockedChapters, int availableLevels)
        {
            UserData data = UserData.Instance;
            Preferences.GameSave.Clear();
            try
            {
                data.UnlockedChapters = unlockedChapters;
                // Even previously saved Mango progress cannot bypass the chapter gate.
                data.SetUnlockedLevelsChapter(20, Constants.BonusChapter);
                Assert.Equal(availableLevels, data.AvailableBonusLevels);
                for (int i = 0; i < 20; i++)
                {
                    Assert.Equal(i >= availableLevels,
                        new LevelPosition(Constants.BonusChapter, i).LockedByChapterProgress);
                    Assert.Equal(i < availableLevels - 1,
                        new LevelPosition(Constants.BonusChapter, i).SkipAvailable);
                }
                Assert.True(new LevelPosition(Constants.NewFriendChapter, 4).SkipAvailable);
                Assert.False(new LevelPosition(Constants.NewFriendChapter, 9).LockedByChapterProgress);
                Assert.True(new LevelPosition(0, 4).SkipAvailable);
                Assert.False(LevelPosition.EndGame.SkipAvailable);
            }
            finally
            {
                Preferences.GameSave.Clear();
            }
        }

        [Fact]
        public void MangoFollowsNewFriendWithRekindledLevelOrder()
        {
            Assert.Equal(7, Constants.ChaptersCount);
            Assert.Equal(130, ContreJourDXConstants.LevelCount);
            Assert.Equal(6, Constants.NormalChaptersCount);
            Assert.Equal(7, LevelsMenu.LevelsList.Count);
            Assert.Equal(MangoLevels, LevelsMenu.LevelsList[6]);
            Assert.Equal(130, LevelsMenu.LevelsList.SelectMany(chapter => chapter).Distinct().Count());
            Assert.Equal(Enumerable.Range(300, 10), LevelsMenu.LevelsList[5]);
            Assert.Equal(6, LevelsMenu.GetLevelPosition(199).Chapter);
            Assert.Equal(19, LevelsMenu.GetLevelPosition(199).Index);
            Assert.Equal(139, LevelsMenu.GetLevelPosition(199).GlobalPosition());
        }

        [Fact]
        public void TotalsIncludeAllMangoLevelsWithoutCountingUnusedNewFriendSlots()
        {
            Preferences.GameSave.Clear();
            try
            {
                UserData data = UserData.Instance;
                data.SetLevelData(new LevelData(1000, 1), 99);
                data.SetLevelData(new LevelData(2000, 2), 109);
                // The unused slots must not become playable levels or affect totals.
                data.SetLevelData(new LevelData(9999, 3), 110);
                for (int i = 0; i < 20; i++)
                {
                    data.SetLevelData(new LevelData(3000, 3), 120 + i);
                }
                Assert.Equal(60, data.GetChapterStars(6));
                Assert.Equal(60000, data.GetChapterScore(6));
                Assert.Equal(63, data.TotalStars);
                Assert.Equal(63000, data.TotalScore);
                Assert.True(data.GetCompleted(6));
                Assert.True(data.GetPerfect(6));
                Assert.True(Preferences.GameSave.Contains("SCORE_6_19"));
                Assert.True(Preferences.GameSave.Contains("SCORE_5_9"));
                Assert.True(Preferences.GameSave.Contains("SCORE_4_19"));
            }
            finally
            {
                Preferences.GameSave.Clear();
            }
        }

        [Theory]
        [InlineData("McBackgroundContent6_1", ClipKind.Sprite)]
        [InlineData("McHeroView_6", ClipKind.Sprite)]
        [InlineData("McEggView1_6", ClipKind.Sprite)]
        [InlineData("McSpringView_6", ClipKind.MovieClip)]
        [InlineData("McRotatableSpring_6", ClipKind.MovieClip)]
        [InlineData("McCircleSpikesView_6", ClipKind.MovieClip)]
        [InlineData("McSpikesView_6", ClipKind.MovieClip)]
        [InlineData("McGrass_6", ClipKind.MovieClip)]
        [InlineData("McSnotPoint", ClipKind.Sprite)]
        [InlineData("McMoveHintView", ClipKind.Sprite)]
        public void GreenThemeAssetsResolveToMangoAtlas(string name, ClipKind kind)
        {
            ClipEntry entry = ClipCatalog.Find(name);
            Assert.NotNull(entry);
            Assert.Equal("chapter6/" + name, entry.Id);
            Assert.Equal(kind, entry.Kind);
        }

        [Theory]
        [InlineData("FontColors")]
        [InlineData("BackColors")]
        public void MenuIncludesMangoGreenAfterNewFriend(string paletteName)
        {
            FieldInfo field = typeof(MainMenu).GetField(paletteName, BindingFlags.NonPublic | BindingFlags.Static);
            Color[] palette = Assert.IsType<Color[]>(field.GetValue(null));
            Assert.Equal(Constants.ChaptersCount, palette.Length);
            Assert.Equal(ContreJourDXConstants.NewFriendColor, palette[5]);
            // Rekindled and the Windows 8 source tint the Mango menu with 0xBFDD00.
            Assert.Equal(new Color(191, 221, 0), palette[6]);
            Assert.Equal("chapter6/McBackgroundContent6_1", MainMenu.Backgrounds()[6]);
        }
    }
}
