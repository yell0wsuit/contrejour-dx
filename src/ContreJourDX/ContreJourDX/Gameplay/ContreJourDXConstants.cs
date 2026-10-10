using System.Linq;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;

namespace ContreJourDX.Gameplay
{
    public static class ContreJourDXConstants
    {
        public const string ViewType = "viewType";

        public const string LowMemoryBackgroundFormat = "{0}Chapter16Bit";

        public const int MinimumTouchCount = 2;

        public const float EndShakeTime = 8f;

        public const string AssemblyName = "ContreJourDX.Win8";

        public const float HeroRadiusPixels = 25f;

        public const int AchievementDeathCount = 50;

        public const float IphoneMultV = 5f / 6f;

        public const float IphoneMultH = 0.9375f;

        public const double CrystalAppVersion = 1.100000023841858;

        public const float HeroDensity = 0.1f;

        public const float SnotDensity = 0.13f;

        public const float FontColorMult = 0.5f;

        public const float SunriseTime = 60f;

        public const float MaxLevelTime = 60f;

        public const float TimeScore = 2000f;

        public const int StarScore = 1000;

        public const float AnimationIntervalLow = 1f / 30f;

        public const float AnimationInterval = 1f / 60f;

        public const float FinishDuration = 0.8f;

        public const int EnergyInLevel = 3;

        public const int BackFog = -3;

        public const int DragBackgroundCircle = -1;

        public const int PortalLayer = -2;

        public const int CURSOR = 20;

        public const int FinishMenu = 16;

        public const int OUTRO = 15;

        public const int PinHole = 14;

        public const int ColorOverlay = 14;

        public const int RestartLayer = 100;

        public const int MENU = 15;

        public const int GroundTop = 1;

        public const int WhiteSnow = -1;

        public const int SNOW = 11;

        public const int HINTS = 12;

        public const int DustLayer = 1;

        public const string FOREGROUNDS = "12";

        public const int FlyLayer = 6;

        public const int BlueParticles = -9;

        public const int PARTICLES = -2;

        public const int EndLevelLayer = 11;

        public const int SPRING = 3;

        public const int SNOT = 4;

        public const int DRAGER = 2;

        public const int ENERGY = 9;

        public const int BlackTails = 3;

        public const int SPRINGS = 2;

        public const int ARROW = -1;

        public const int DragableContainer = -1;

        public const int TrampolinePath = 11;

        public const int HERO = 10;

        public const int FrontLimit = 1;

        public const int GroundFall = -1;

        public const int GRASS = -1;

        public const int LIANA = -3;

        public const int BlackKaktus = -2;

        public const int COLOR = -1;

        public const int BackLimit = -3;

        public const int BackSnotLayer = -9;

        public const int HeroesGroup = -2;

        public const int SUCKER = 1;

        public const int MinSnotParts = 3;

        public const int CcContentScaleFactor = 1;

        public const float EndLightsMultiplier = 1f / 3f;

        public const int GameIndex = 0;

        public const int EndLevel = 169;

        public static readonly int PlanetsCount = Constants.IsTrial ? (Constants.ChaptersCount + 1) : Constants.ChaptersCount;

        public static readonly string[] CrystalKeys = ["ln4gjvlcteqceld7djbntu67c0saqg", "5ij7a8rvvgla4ti7j5fa64dlheqerp"];

        public static readonly string[] CrystalId = ["1678091788", "1920152455"];

        public static readonly string[] BlocksDestroy = ["1700403143", "1920121598"];

        public static readonly string[] FeedMonsters = ["1700216187", "1920339314"];

        public static readonly string[] TrampolineShot = ["1700312770", "1920297830"];

        public static readonly string[] SpringShot = ["1700338609", "1920303624"];

        public static readonly string[] SnotEyeHit = ["1700205971", "1920220952"];

        public static readonly string[] SPIDER = ["1700181564", "1920302318"];

        public static readonly string[] ACUPUNCTURE = ["1700224152", "1920199911"];

        public static readonly string[] OutOfScreen = ["1700179765", "1920065907"];

        public static readonly string[] FastPerfect = ["1700206601", "1920349029"];

        public static readonly string[] RushHour = ["1700245981", "1920240891"];

        public static readonly string[] MightyBird = ["1700201897", "1920327325"];

        public static readonly string[] SPEEDY = ["1700167913", "1920332112"];

        public static readonly string[] Collect240LightsId = ["2420410513", "2420327477"];

        public static readonly string[] Collect180LightsId = ["1700248039", "1920244490"];

        public static readonly string[] Collect90LightsId = ["1700233166", "1920309206"];

        public static readonly string[,] ChapterPerfect = new string[6, 2]
        {
            { "1700180231", "1920257716" },
            { "1700086869", "1920172995" },
            { "1700176825", "1920320074" },
            { "1700086869", "1920172995" },
            { "1700176825", "1920320074" },
            { "2420316885", "2420334777" }
        };

        public static readonly string[,] ChapterComplete = new string[6, 2]
        {
            { "1700171375", "1920065801" },
            { "1700179404", "1920228565" },
            { "1700072945", "1920264540" },
            { "1700179404", "1920228565" },
            { "1700072945", "1920264540" },
            { "2420411375", "2420416250" }
        };

        public static readonly string[] LeaderboardTotal = ["1679624421", "1920358517"];

        public static readonly string[,] LEADERBOARDS = new string[5, 2]
        {
            { "1679485460", "1920370288" },
            { "1679543437", "1920324399" },
            { "1679543437", "1920324399" },
            { "1679553562", "1920277486" },
            { "2419793981", "2420147133" }
        };

        public static readonly Color WhiteTailColor = 9211020.ToRGBColor();

        public static readonly Color WhiteSnotEndColor = 13421772.ToRGBColor();

        public static readonly Color WhiteSnotStartColor = 10526880.ToRGBColor();

        public static readonly Color GreyColor = new(0.5882353f, 0.5882353f, 0.5882353f);

        public static readonly Color BlueLightColor = 1721955.ToRGBColor();

        public static readonly Color NewFriendColor = new(128, 108, 141);

        public static readonly Color GreenLightColor = 12253975.ToRGBColor();

        public static readonly Color GreenSnotStart = 12844819.ToRGBColor();

        public static readonly Color GreenSnotEnd = 12844819.ToRGBColor();

        public static readonly Color GreenTail = GreenLightColor * 0.7f;

        public static readonly Color GreenSpikesFlower = 6790656.ToRGBColor();

        public static readonly Color WhiteLightColor = new(14f / 51f, 14f / 51f, 14f / 51f);

        public static readonly int LevelCount = LevelsMenu.LevelsList.Sum(chapter => chapter.Count);

        public static readonly string[] AppUrls = ["http://itunes.apple.com/app/id440693481", "http://itunes.apple.com/app/id444085845?mt=8"];

        public static float GetAnimationInterval()
        {
            return 1f / 60f;
        }
    }
}
