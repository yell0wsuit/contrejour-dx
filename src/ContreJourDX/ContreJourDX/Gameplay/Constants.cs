namespace ContreJourDX.Gameplay
{
    public static class Constants
    {
        public const string CONFIG = "config";

        public const string TYPE = "type";

        public const string POSITION = "position";

        public const float PRECISION = 1E-10f;

        public const int NewFriendChapter = 5;

        public const int BonusChapter = 6;

        public const int BlueChapter = 1;

        public const int WhiteChapter = 3;

        public const int RoseChapter = 4;

        public const int LevelsInChapter = 20;

        public const int TrialLevelsInChapter = 10;

        public const int KGameAutorotationNone = 0;

        public const int KGameAutorotationCcDirector = 1;

        public const int KGameAutorotationUiViewController = 2;

        public static readonly int ChaptersCount = IsTrial ? 2 : 7;

        public static readonly int NormalChaptersCount = IsTrial ? 2 : 6;

        public static bool IsTrial => false;

        public static int LevelsToPlay => !IsTrial ? 20 : 10;
    }
}
