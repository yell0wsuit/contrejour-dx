using System.Linq;

using ContreJour.Gameplay;
using ContreJour.Saving;

using Xunit;

namespace ContreJour.Tests
{
    public class NewFriendChapterTests
    {
        [Fact]
        public void NewFriendFollowsJungleWithTenIndependentLevels()
        {
            Assert.Equal(7, Constants.ChaptersCount);
            Assert.Equal(130, ContreJourConstants.LevelCount);
            Assert.Equal(240, UserData.StarsToUnlock(5));
            Assert.Equal(7, LevelsMenu.LevelsList.Count);
            Assert.Equal(Enumerable.Range(300, 10), LevelsMenu.LevelsList[5]);
            Assert.Equal(130, LevelsMenu.LevelsList.Sum(chapter => chapter.Count));
            Assert.Equal(130, LevelsMenu.LevelsList.SelectMany(chapter => chapter).Distinct().Count());
            Assert.Equal(149, LevelsMenu.GetLevelIndex(new LevelPosition(4, 0)));
            Assert.Equal(134, LevelsMenu.GetLevelIndex(new LevelPosition(4, 19)));
            for (int i = 0; i < 10; i++)
            {
                LevelPosition position = LevelsMenu.GetLevelPosition(300 + i);
                Assert.Equal(5, position.Chapter);
                Assert.Equal(i, position.Index);
                Assert.Equal(100 + i, position.GlobalPosition());
            }
        }

        [Fact]
        public void ChapterCompletionUsesItsPlayableLevelsAndKeepsSaveKeys()
        {
            Preferences.GameSave.Clear();
            try
            {
                UserData data = UserData.Instance;
                for (int i = 0; i < 9; i++)
                {
                    data.SetLevelData(new LevelData(3000, 3), 100 + i);
                }
                Assert.False(data.GetCompleted(5));
                data.SetLevelData(new LevelData(3000, 3), 109);
                Assert.True(data.GetCompleted(5));
                Assert.True(data.GetPerfect(5));
                Assert.Equal(30, data.GetChapterStars(5));
                Assert.Equal(30000, data.GetChapterScore(5));
                Assert.True(Preferences.GameSave.Contains("SCORE_5_9"));
                Assert.False(Preferences.GameSave.Contains("SCORE_5_10"));
                data.SetLevelData(new LevelData(1000, 1), 99);
                Assert.Equal(1, data.GetLevelDataByPosition(new LevelPosition(4, 19)).StarsCount);
                Assert.Equal(31, data.TotalStars);
            }
            finally
            {
                Preferences.GameSave.Clear();
            }
        }
    }
}
