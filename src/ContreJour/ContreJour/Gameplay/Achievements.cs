using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay
{
    public static class Achievements
    {
        public const string Speedy = "speedy";

        public const string MightyBird = "mighty_bird";

        public const string RushHour = "rush_hour";

        public const string FastPerfect = "fast_perfect";

        public const string Spider = "spider";

        public const string BlueLantern = "blue_lantern";

        public const string Sunrise = "sunrise";

        public const string LittlePrince = "little_prince";

        public static string GetChapterPerfect(int chapter)
        {
            return "chapter{0}_perfect".FormatThis(chapter + 1);
        }
    }
}
