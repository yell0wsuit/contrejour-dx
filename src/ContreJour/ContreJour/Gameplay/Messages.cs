using System.Text;
using System.Text.RegularExpressions;

using Mokus2D.Localization;
using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay
{
    public static class Messages
    {
        private static readonly LocalizationBundle Bundle = new("values");

        private static readonly LocalizationBundle DefaultBundle = new("values", string.Empty);

        public static readonly string LEVEL = "LEVEL".Localize();

        public static readonly string BestScore = "BEST_SCORE".Localize();

        public static readonly string EnergyBonus = "ENERGY_BONUS".Localize();

        public static readonly string TimeBonus = "TIME_BONUS".Localize();

        public static readonly string TOTAL = "TOTAL".Localize();

        public static readonly string StarsAndScore = "STARS_AND_SCORE".Localize();

        public static readonly CompositeFormat LevelFormat = CompositeFormat.Parse(LEVEL);

        public static readonly CompositeFormat StarsAndScoreFormat = CompositeFormat.Parse(StarsAndScore);

        public static string CompleteText(int stars)
        {
            return "COMPLETE_TEXT_{0}".FormatThis(stars).Localize();
        }

        public static string Localize(this string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return string.Empty;
            }
            string text = Bundle.GetLocalizedMessage(id) ?? DefaultBundle.GetLocalizedMessage(id);
            return string.IsNullOrEmpty(text) ? id : Regex.Replace(text.Trim(), "\\n\\s*", "\n");
        }
    }
}
