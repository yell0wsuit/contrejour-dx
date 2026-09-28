namespace Default.Namespace;

public static class Constants
{
    public const string CONFIG = "config";

    public const string TYPE = "type";

    public const string POSITION = "position";

    public const float PRECISION = 1E-10f;

    public const int BonusChapter = 5;

    public const int BlueChapter = 1;

    public const int WhiteChapter = 3;

    public const int RoseChapter = 4;

    public const int LEVELS_IN_CHAPTER = 20;

    public const int TrialLevelsInChapter = 10;

    public const int K_GAME_AUTOROTATION_NONE = 0;

    public const int K_GAME_AUTOROTATION_CC_DIRECTOR = 1;

    public const int K_GAME_AUTOROTATION_UI_VIEW_CONTROLLER = 2;

    public static readonly int ChaptersCount = IsTrial ? 2 : 5;

    public static readonly int NormalChaptersCount = IsTrial ? 2 : 5;

    public static bool IsTrial => false;

    public static int LevelsToPlay => !IsTrial ? 20 : 10;
}
