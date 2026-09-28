using System;
using System.IO;
using System.Xml.Serialization;
using Mokus2D.Sound;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class UserData
{
    private const string FILE_NAME = "contreJourData.xml";

    private const int ENJOY_STARS = 30;

    private const int ROSE_NEEDED_LIGHTS = 240;

    private const int MinUnlockedChapters = 1;

    private static readonly int[] STARS_TO_UNLOCK = new int[5] { 0, 30, 70, 120, 180 };

    private static UserData instance;

    private static readonly XmlSerializer serializer = new XmlSerializer(typeof(UserData));

    private static bool levelPostponed;

    private static LevelPosition postponedLevel;

    private LevelData[] levelData = new LevelData[Constants.ChaptersCount * 20];

    private int[] unlockedLevels = new int[Constants.ChaptersCount];

    private int unlockedChapters;

    private bool _soundDisabled;

    private bool _musicDisabled;

    public static UserData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = ReadUserData();
            }
            return instance;
        }
    }

    public LevelData[] LevelData
    {
        get
        {
            return levelData;
        }
        set
        {
            for (int i = 0; i < value.Length && i < levelData.Length; i++)
            {
                levelData[i] = value[i];
            }
        }
    }

    public int[] UnlockedLevels
    {
        get
        {
            return unlockedLevels;
        }
        set
        {
            for (int i = 0; i < value.Length && i < unlockedLevels.Length; i++)
            {
                unlockedLevels[i] = value[i];
            }
        }
    }

    public bool SoundDisabled
    {
        get
        {
            return _soundDisabled;
        }
        set
        {
            if (_soundDisabled != value)
            {
                _soundDisabled = value;
                SaveUserData();
            }
        }
    }

    public bool MusicDisabled
    {
        get
        {
            return _musicDisabled;
        }
        set
        {
            if (_musicDisabled != value)
            {
                _musicDisabled = value;
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
        get
        {
            return Math.Max(unlockedChapters, 1);
        }
        set
        {
            unlockedChapters = Math.Max(1, value);
        }
    }

    public int TotalStars => GetStarsEnd(0, ContreJourConstants.LEVEL_COUNT);

    public int TotalScore => GetScoreEnd(0, ContreJourConstants.LEVEL_COUNT);

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
        return STARS_TO_UNLOCK[chapter];
    }

    public static int GetTimeBonus(float time)
    {
        return (int)Math.Max(2000f * (180f - time) / 180f, 0f);
    }

    public void SetUnlockedLevelsChapter(int value, int chapter)
    {
        unlockedLevels[chapter] = value;
    }

    public int GetUnlockedLevels(int chapter)
    {
        return unlockedLevels[chapter];
    }

    public void UnlockChapter(int chapter)
    {
        UnlockedChapters = Math.Max(chapter + 1, unlockedChapters);
    }

    public string TotalStarsString()
    {
        return TotalStars.ToString();
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
        for (int i = start; i < Math.Min(this.levelData.Length, end); i++)
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
        for (int i = start; i < Math.Min(this.levelData.Length, end); i++)
        {
            LevelData levelData = GetLevelData(i);
            if (levelData != null)
            {
                num += levelData.Score;
            }
        }
        return num;
    }

    public LevelPosition GetLevelPosition(int index)
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
        levelData[index] = data;
    }

    public LevelData GetLevelData(int index)
    {
        return levelData[index];
    }

    public void CompleteAll()
    {
        for (int i = 0; i < Constants.ChaptersCount * 20; i++)
        {
            CompleteLevel(new LevelPosition(i / 20, i % 20), 2, 100f);
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
        int num = GetTimeBonus(time) + stars * 1000;
        RefreshHighscores = true;
        bool flag = num > levelData.Score;
        bool flag2 = stars > levelData.StarsCount;
        if (flag || flag2)
        {
            SetLevelData(new LevelData(Math.Max(num, levelData.Score), Math.Max(stars, levelData.StarsCount)), position.GlobalPosition());
            if (flag2)
            {
                this.TotalStarsChanged.Dispatch(TotalStars);
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

    private void SaveHighscore()
    {
    }

    public void PostLevelAchievements()
    {
        int totalStars = TotalStars;
        if (totalStars >= 90 && totalStars >= 180)
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

    private static string DataFilePath => Path.Combine(DataDirectory, FILE_NAME);

    private static UserData ReadUserData()
    {
        try
        {
            byte[] array = File.ReadAllBytes(DataFilePath);
            byte[] buffer = CryptUtils.RunProtector(array, encrypt: false);
            return ((UserData)serializer.Deserialize(new MemoryStream(buffer))) ?? new UserData();
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
            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, instance);
            Directory.CreateDirectory(Path.GetDirectoryName(DataFilePath));
            File.WriteAllBytes(DataFilePath, CryptUtils.RunProtector(memoryStream.ToArray(), encrypt: true));
        }
    }
}
