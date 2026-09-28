using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public static class ContreJourConstants
{
    public const string ViewType = "viewType";

    public const string LowMemoryBackgroundFormat = "{0}Chapter16Bit";

    public const int MinimumTouchCount = 2;

    public const float END_SHAKE_TIME = 8f;

    public const string ASSEMBLY_NAME = "ContreJour.Win8";

    public const string NAMESPACE_PREFIX = "Default.Namespace.";

    public const float HERO_RADIUS_PIXELS = 25f;

    public const int ACHIEVEMENT_DEATH_COUNT = 50;

    public const float IPHONE_MULT_V = 5f / 6f;

    public const float IPHONE_MULT_H = 0.9375f;

    public const double CRYSTAL_APP_VERSION = 1.100000023841858;

    public const float HERO_DENSITY = 0.1f;

    public const float SNOT_DENSITY = 0.13f;

    public const float FONT_COLOR_MULT = 0.5f;

    public const float SUNRISE_TIME = 60f;

    public const float MAX_LEVEL_TIME = 60f;

    public const float TIME_SCORE = 2000f;

    public const int STAR_SCORE = 1000;

    public const float ANIMATION_INTERVAL_LOW = 1f / 30f;

    public const float ANIMATION_INTERVAL = 1f / 60f;

    public const float FINISH_DURATION = 0.8f;

    public const int ENERGY_IN_LEVEL = 3;

    public const int BACK_FOG = -3;

    public const int DRAG_BACKGROUND_CIRCLE = -1;

    public const int PORTAL_LAYER = -2;

    public const int CURSOR = 20;

    public const int FINISH_MENU = 16;

    public const int OUTRO = 15;

    public const int PIN_HOLE = 14;

    public const int COLOR_OVERLAY = 14;

    public const int RESTART_LAYER = 100;

    public const int MENU = 15;

    public const int GROUND_TOP = 1;

    public const int WHITE_SNOW = -1;

    public const int SNOW = 11;

    public const int HINTS = 12;

    public const int DUST_LAYER = 1;

    public const string FOREGROUNDS = "12";

    public const int FLY_LAYER = 6;

    public const int BLUE_PARTICLES = -9;

    public const int PARTICLES = -2;

    public const int END_LEVEL_LAYER = 11;

    public const int SPRING = 3;

    public const int SNOT = 4;

    public const int DRAGER = 2;

    public const int ENERGY = 9;

    public const int BLACK_TAILS = 3;

    public const int SPRINGS = 2;

    public const int ARROW = -1;

    public const int DRAGABLE_CONTAINER = -1;

    public const int TRAMPOLINE_PATH = 11;

    public const int HERO = 10;

    public const int FRONT_LIMIT = 1;

    public const int GROUND_FALL = -1;

    public const int GRASS = -1;

    public const int LIANA = -3;

    public const int BLACK_KAKTUS = -2;

    public const int COLOR = -1;

    public const int BACK_LIMIT = -3;

    public const int BACK_SNOT_LAYER = -9;

    public const int HEROES_GROUP = -2;

    public const int SUCKER = 1;

    public const int MIN_SNOT_PARTS = 3;

    public const int CC_CONTENT_SCALE_FACTOR = 1;

    public const float END_LIGHTS_MULTIPLIER = 1f / 3f;

    public const int GAME_INDEX = 0;

    public const int END_LEVEL = 169;

    public static readonly int PlanetsCount = Constants.IsTrial ? (Constants.ChaptersCount + 1) : Constants.ChaptersCount;

    public static string[] CRYSTAL_KEYS = ["ln4gjvlcteqceld7djbntu67c0saqg", "5ij7a8rvvgla4ti7j5fa64dlheqerp"];

    public static string[] CRYSTAL_ID = ["1678091788", "1920152455"];

    public static string[] BLOCKS_DESTROY = ["1700403143", "1920121598"];

    public static string[] FEED_MONSTERS = ["1700216187", "1920339314"];

    public static string[] TRAMPOLINE_SHOT = ["1700312770", "1920297830"];

    public static string[] SPRING_SHOT = ["1700338609", "1920303624"];

    public static string[] SNOT_EYE_HIT = ["1700205971", "1920220952"];

    public static string[] SPIDER = ["1700181564", "1920302318"];

    public static string[] ACUPUNCTURE = ["1700224152", "1920199911"];

    public static string[] OUT_OF_SCREEN = ["1700179765", "1920065907"];

    public static string[] FAST_PERFECT = ["1700206601", "1920349029"];

    public static string[] RUSH_HOUR = ["1700245981", "1920240891"];

    public static string[] MIGHTY_BIRD = ["1700201897", "1920327325"];

    public static string[] SPEEDY = ["1700167913", "1920332112"];

    public static string[] COLLECT_240_LIGHTS_ID = ["2420410513", "2420327477"];

    public static string[] COLLECT_180_LIGHTS_ID = ["1700248039", "1920244490"];

    public static string[] COLLECT_90_LIGHTS_ID = ["1700233166", "1920309206"];

    public static readonly string[,] CHAPTER_PERFECT = new string[6, 2]
    {
        { "1700180231", "1920257716" },
        { "1700086869", "1920172995" },
        { "1700176825", "1920320074" },
        { "1700086869", "1920172995" },
        { "1700176825", "1920320074" },
        { "2420316885", "2420334777" }
    };

    public static readonly string[,] CHAPTER_COMPLETE = new string[6, 2]
    {
        { "1700171375", "1920065801" },
        { "1700179404", "1920228565" },
        { "1700072945", "1920264540" },
        { "1700179404", "1920228565" },
        { "1700072945", "1920264540" },
        { "2420411375", "2420416250" }
    };

    public static string[] LEADERBOARD_TOTAL = ["1679624421", "1920358517"];

    public static readonly string[,] LEADERBOARDS = new string[5, 2]
    {
        { "1679485460", "1920370288" },
        { "1679543437", "1920324399" },
        { "1679543437", "1920324399" },
        { "1679553562", "1920277486" },
        { "2419793981", "2420147133" }
    };

    public static readonly Color WHITE_TAIL_COLOR = 9211020.ToRGBColor();

    public static readonly Color WHITE_SNOT_END_COLOR = 13421772.ToRGBColor();

    public static readonly Color WHITE_SNOT_START_COLOR = 10526880.ToRGBColor();

    public static readonly Color GREY_COLOR = new(0.5882353f, 0.5882353f, 0.5882353f);

    public static readonly Color BLUE_LIGHT_COLOR = 1721955.ToRGBColor();

    public static readonly Color GreenLightColor = 12253975.ToRGBColor();

    public static readonly Color GreenSnotStart = 12844819.ToRGBColor();

    public static readonly Color GreenSnotEnd = 12844819.ToRGBColor();

    public static readonly Color GreenTail = GreenLightColor * 0.7f;

    public static readonly Color GreenSpikesFlower = 6790656.ToRGBColor();

    public static readonly Color WHITE_LIGHT_COLOR = new(14f / 51f, 14f / 51f, 14f / 51f);

    public static readonly int LEVEL_COUNT = Constants.ChaptersCount * 20;

    public static string[] APP_URLS = ["http://itunes.apple.com/app/id440693481", "http://itunes.apple.com/app/id444085845?mt=8"];

    public static float GetAnimationInterval()
    {
        return 1f / 60f;
    }
}
