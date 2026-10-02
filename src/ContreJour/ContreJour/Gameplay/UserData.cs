using System;
using System.Globalization;

using ContreJour.Saving;
using ContreJour.Utils;

using Mokus2D.Sound;
using Mokus2D.Util;

namespace ContreJour.Gameplay
{
    /// <summary>
    /// The player's settings and progress, stored by <see cref="Preferences"/>: settings in the
    /// preferences file, and unlocks, scores, stars and stats for every chapter in the game save.
    /// </summary>
    public class UserData
    {
        private const int LevelsPerChapter = 20;

        private const string SoundOnKey = "SOUND_ON";
        private const string MusicOnKey = "MUSIC_ON";
        private const string IntroWatchedKey = "PREFS_INTRO_WATCHED";

        private const string UnlockedChaptersKey = "UNLOCKED_CHAPTERS";
        private const string UnlockedLevelsKey = "UNLOCKED_LEVELS_";
        private const string ScoreKey = "SCORE_";
        private const string StarsKey = "STARS_";
        private const string OutOfScreenKey = "STAT_OUT_OF_SCREEN";
        private const string SpringShotKey = "STAT_SPRING_SHOT";
        private const string AccupunctureKey = "STAT_ACCUPUNCTURE";
        private const string FeedMonsterKey = "STAT_FEED_MONSTER";
        private const string BlocksDestroyedKey = "STAT_BLOCKS_DESTROYED";

        private static readonly int[] StarsToUnlockByChapter = [0, 30, 70, 120, 180];
        private static LevelPosition postponedLevel;

        public static UserData Instance
        {
            get
            {
                if (field == null)
                {
                    Preferences.Load();
                    field = new UserData();
                }
                return field;
            }
        }

        // The same file objects for the whole session: Preferences.Load reloads them in place.
        private readonly PreferenceFile settings = Preferences.Settings;

        private readonly PreferenceFile gameSave = Preferences.GameSave;

        public bool SoundDisabled
        {
            get => !settings.GetBool(SoundOnKey, true);
            set
            {
                if (SoundDisabled != value)
                {
                    settings.SetBool(SoundOnKey, !value);
                    Preferences.RequestSave();
                }
            }
        }

        public bool MusicDisabled
        {
            get => !settings.GetBool(MusicOnKey, true);
            set
            {
                if (MusicDisabled != value)
                {
                    settings.SetBool(MusicOnKey, !value);
                    Preferences.RequestSave();
                }
            }
        }

        public bool IntroWatched
        {
            get => settings.GetBool(IntroWatchedKey);
            set => settings.SetBool(IntroWatchedKey, value);
        }

        public bool LastLevelOpen => GetLevelDataByPosition(new LevelPosition(4, 19)) != null;

        public int OutOfScreen
        {
            get => gameSave.GetInt(OutOfScreenKey);
            set => gameSave.SetInt(OutOfScreenKey, value);
        }

        public int SpringShot
        {
            get => gameSave.GetInt(SpringShotKey);
            set => gameSave.SetInt(SpringShotKey, value);
        }

        public int Accupuncture
        {
            get => gameSave.GetInt(AccupunctureKey);
            set => gameSave.SetInt(AccupunctureKey, value);
        }

        public int FeedMonster
        {
            get => gameSave.GetInt(FeedMonsterKey);
            set => gameSave.SetInt(FeedMonsterKey, value);
        }

        public int BlocksDestroyed
        {
            get => gameSave.GetInt(BlocksDestroyedKey);
            set => gameSave.SetInt(BlocksDestroyedKey, value);
        }

        public int UnlockedChapters
        {
            get => Math.Max(gameSave.GetInt(UnlockedChaptersKey), 1);
            set => gameSave.SetInt(UnlockedChaptersKey, Math.Max(1, value));
        }

        public int TotalStars => GetStarsEnd(0, ContreJourConstants.LevelCount);

        public int TotalScore => GetScoreEnd(0, ContreJourConstants.LevelCount);

        public bool RoseSaved => TotalStars >= 240;

        public event Action<int> TotalStarsChanged;

        private UserData()
        {
            SoundManager.MusicDisableEvent += OnMusicDisable;
            InitializeDefaults();
        }

        // First launch: write the settings' defaults, so the preferences file lists them.
        private void InitializeDefaults()
        {
            if (!settings.Contains(SoundOnKey))
            {
                settings.SetBool(SoundOnKey, true);
            }
            if (!settings.Contains(MusicOnKey))
            {
                settings.SetBool(MusicOnKey, true);
            }
            if (settings.Dirty)
            {
                Preferences.RequestSave();
            }
        }

        private void OnMusicDisable()
        {
            MusicDisabled = true;
        }

        public void RefreshSoundManager()
        {
            SoundManager.MusicEnabled = !MusicDisabled && SoundManager.HasControl;
            SoundManager.SoundEnabled = !SoundDisabled;
        }

        public static int StarsToUnlock(int chapter)
        {
            return StarsToUnlockByChapter[chapter];
        }

        public static int GetTimeBonus(float time)
        {
            return (int)Math.Max(2000f * (180f - time) / 180f, 0f);
        }

        public void SetUnlockedLevelsChapter(int value, int chapter)
        {
            gameSave.SetInt(UnlockedLevelsKey + chapter.ToString(CultureInfo.InvariantCulture), value);
        }

        public int GetUnlockedLevels(int chapter)
        {
            return gameSave.GetInt(UnlockedLevelsKey + chapter.ToString(CultureInfo.InvariantCulture));
        }

        public void UnlockChapter(int chapter)
        {
            UnlockedChapters = Math.Max(chapter + 1, gameSave.GetInt(UnlockedChaptersKey));
        }

        public string TotalStarsString()
        {
            return TotalStars.ToString(CultureInfo.CurrentCulture);
        }

        public int GetChapterStars(int chapter)
        {
            return GetStarsEnd(chapter * 20, (chapter + 1) * 20);
        }

        public int GetChapterScore(int chapter)
        {
            return GetScoreEnd(chapter * 20, (chapter + 1) * 20);
        }

        public bool GetCompleted(int chapter)
        {
            for (int i = chapter * 20; i < (chapter + 1) * 20; i++)
            {
                if (GetLevelData(i) == null)
                {
                    return false;
                }
            }
            return true;
        }

        public bool GetPerfect(int chapter)
        {
            for (int i = chapter * 20; i < (chapter + 1) * 20; i++)
            {
                LevelData levelData = GetLevelData(i);
                if (levelData == null || levelData.StarsCount < 3)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetStarsEnd(int start, int end)
        {
            int num = 0;
            for (int i = start; i < Math.Min(ContreJourConstants.LevelCount, end); i++)
            {
                LevelData levelData = GetLevelData(i);
                if (levelData != null)
                {
                    num += levelData.StarsCount;
                }
            }
            return num;
        }

        public int GetScoreEnd(int start, int end)
        {
            int num = 0;
            for (int i = start; i < Math.Min(ContreJourConstants.LevelCount, end); i++)
            {
                LevelData levelData = GetLevelData(i);
                if (levelData != null)
                {
                    num += levelData.Score;
                }
            }
            return num;
        }

        public static LevelPosition GetLevelPosition(int index)
        {
            return LevelsMenu.GetLevelPosition(index);
        }

        public LevelData GetLevelDataByFile(int index)
        {
            return GetLevelData(LevelsMenu.GetLevelPosition(index).GlobalPosition());
        }

        public LevelData GetLevelDataByPosition(LevelPosition position)
        {
            return GetLevelData(position.GlobalPosition());
        }

        public void SetLevelData(LevelData data, int index)
        {
            string key = LevelKey(index);
            if (data == null)
            {
                gameSave.Remove(ScoreKey + key);
                gameSave.Remove(StarsKey + key);
                return;
            }
            gameSave.SetInt(ScoreKey + key, data.Score);
            gameSave.SetInt(StarsKey + key, data.StarsCount);
        }

        // A level has data once it has been completed; its score key marks that.
        public LevelData GetLevelData(int index)
        {
            string key = LevelKey(index);
            return gameSave.Contains(ScoreKey + key) ? new LevelData(gameSave.GetInt(ScoreKey + key), gameSave.GetInt(StarsKey + key)) : null;
        }

        // Chapter and level within the chapter, as in CHAPTER_LEVEL (e.g. "1_4").
        private static string LevelKey(int index)
        {
            return (index / LevelsPerChapter).ToString(CultureInfo.InvariantCulture) + "_" + (index % LevelsPerChapter).ToString(CultureInfo.InvariantCulture);
        }

        public void CompleteAll()
        {
            for (int i = 0; i < Constants.ChaptersCount * 20; i++)
            {
                _ = CompleteLevel(new LevelPosition(i / 20, i % 20), 2, 100f);
            }
            XBoxUtil.AwardAchievement("blue_lantern");
            XBoxUtil.AwardAchievement("fast_perfect");
            XBoxUtil.AwardAchievement(Achievements.GetChapterPerfect(4));
            XBoxUtil.AwardAchievement("little_prince");
            XBoxUtil.AwardAchievement("mighty_bird");
            XBoxUtil.AwardAchievement("rush_hour");
            XBoxUtil.AwardAchievement("speedy");
            XBoxUtil.AwardAchievement("spider");
            XBoxUtil.AwardAchievement("sunrise");
        }

        public void SkipLevel(LevelPosition position)
        {
            if (GetUnlockedLevels(position.Chapter) < position.Index + 1)
            {
                SetUnlockedLevelsChapter(position.Index + 1, position.Chapter);
            }
        }

        public int CompleteLevel(LevelPosition position, int stars, float time)
        {
            SkipLevel(position);
            LevelData levelData = GetLevelDataByPosition(position) ?? new LevelData();
            int num = GetTimeBonus(time) + (stars * 1000);
            bool flag = num > levelData.Score;
            bool flag2 = stars > levelData.StarsCount;
            if (flag || flag2)
            {
                SetLevelData(new LevelData(Math.Max(num, levelData.Score), Math.Max(stars, levelData.StarsCount)), position.GlobalPosition());
                if (flag2)
                {
                    TotalStarsChanged.Dispatch(TotalStars);
                }
                if (flag)
                {
                    SaveHighscore();
                }
            }
            postponedLevel = position;
            PostLevelAchievements();
            return num;
        }

        private static void SaveHighscore()
        {
        }

        public void PostLevelAchievements()
        {
            int totalStars = TotalStars;
            if (totalStars is >= 90 and >= 180)
            {
                XBoxUtil.AwardAchievement("blue_lantern");
                if (totalStars >= 300)
                {
                    XBoxUtil.AwardAchievement("sunrise");
                }
            }
            if (GetCompleted(postponedLevel.Chapter) && GetPerfect(postponedLevel.Chapter))
            {
                XBoxUtil.AwardAchievement(Achievements.GetChapterPerfect(postponedLevel.Chapter));
            }
        }
    }
}
