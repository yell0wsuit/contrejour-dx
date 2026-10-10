using System.Globalization;

namespace ContreJourDX.Gameplay
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
            return string.Format(CultureInfo.InvariantCulture, "chapter{0}_perfect", chapter + 1);
        }
    }
}
