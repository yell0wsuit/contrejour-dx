using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Mokus2D.Sound;
using Mokus2D.Util;
using Mokus2D.Util.Data;

namespace ContreJour.Gameplay;

public class UserData
{
    private const string FileName = "contreJourData.xml";
    private static readonly int[] StarsToUnlockByChapter = [0, 30, 70, 120, 180];

    private static UserData instance;

    private static readonly XmlSerializer serializer = new(typeof(UserData));

    private static LevelPosition postponedLevel;
    private int unlockedChapters;

    public static UserData Instance
    {
        get
        {
            instance ??= ReadUserData();
            return instance;
        }
    }

    public LevelData[] LevelData
    {
        get;
        set
        {
            for (int i = 0; i < value.Length && i < field.Length; i++)
            {
                field[i] = value[i];
            }
        }
    } = new LevelData[Constants.ChaptersCount * 20];

    public int[] UnlockedLevels
    {
        get;
        set
        {
            for (int i = 0; i < value.Length && i < field.Length; i++)
            {
                field[i] = value[i];
            }
        }
    } = new int[Constants.ChaptersCount];

    public bool SoundDisabled
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                SaveUserData();
            }
        }
    }

    public bool MusicDisabled
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                SaveUserData();
            }
        }
    }

    public DateTime EnjoyDate { get; set; }

    public bool IntroWatched { get; set; }

    public bool InstallSent { get; set; }

    public bool Improved { get; set; }

    public bool EnjoyShown { get; set; }

    public bool HasToShowEnjoy
    {
        get
        {
            float num = (DateTime.Now - EnjoyDate).Days;
            if (!EnjoyShown && Improved && TotalStars >= 30)
            {
                if (!(num >= 1f))
                {
                    _ = EnjoyDate;
                    return false;
                }
                return true;
            }
            return false;
        }
    }

    public bool LastLevelOpen => GetLevelDataByPosition(new LevelPosition(4, 19)) != null;

    public int OutOfScreen { get; set; }

    public int SpringShot { get; set; }

    public int TrampolineShot { get; set; }

    public int Accupuncture { get; set; }

    public int FeedMonster { get; set; }

    public int SnotEyeHit { get; set; }

    public int BlocksDestroyed { get; set; }

    public bool RefreshHighscores { get; set; }

    public int UnlockedChapters
    {
        get => Math.Max(unlockedChapters, 1);
        set => unlockedChapters = Math.Max(1, value);
    }

    public int TotalStars => GetStarsEnd(0, ContreJourConstants.LevelCount);

    public int TotalScore => GetScoreEnd(0, ContreJourConstants.LevelCount);

    public bool RoseSaved => TotalStars >= 240;

    public event Action<int> TotalStarsChanged;

    private UserData()
    {
        SoundManager.MusicDisableEvent += OnMusicDisable;
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
        UnlockedLevels[chapter] = value;
    }

    public int GetUnlockedLevels(int chapter)
    {
        return UnlockedLevels[chapter];
    }

    public void UnlockChapter(int chapter)
    {
        UnlockedChapters = Math.Max(chapter + 1, unlockedChapters);
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
        for (int i = start; i < Math.Min(LevelData.Length, end); i++)
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
        for (int i = start; i < Math.Min(LevelData.Length, end); i++)
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
        LevelData[index] = data;
    }

    public LevelData GetLevelData(int index)
    {
        return LevelData[index];
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
        RefreshHighscores = true;
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

    public static string DataDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ContreJour");

    private static string DataFilePath => Path.Combine(DataDirectory, FileName);

    private static UserData ReadUserData()
    {
        try
        {
            byte[] array = File.ReadAllBytes(DataFilePath);
            byte[] buffer = CryptUtils.RunProtector(array);
            using XmlReader reader = XmlReader.Create(new MemoryStream(buffer));
            return ((UserData)serializer.Deserialize(reader)) ?? new UserData();
        }
        catch (Exception)
        {
            return new UserData();
        }
    }

    public static void SaveUserData()
    {
        if (instance != null)
        {
            MemoryStream memoryStream = new();
            serializer.Serialize(memoryStream, instance);
            _ = Directory.CreateDirectory(Path.GetDirectoryName(DataFilePath));
            File.WriteAllBytes(DataFilePath, CryptUtils.RunProtector(memoryStream.ToArray()));
        }
    }
}
