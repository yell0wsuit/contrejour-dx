using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

using ContreJourDX.Gameplay;

using Mokus2D.Localization;
using Mokus2D.Visual.Text;

namespace ContreJourDX.Utils
{
    public static class ContreJourDXLabelUtil
    {
        private static readonly List<string> SmallAsianLanguages = ["ja", "ko"];

        private static readonly List<string> Locales = ["de", "es", "fr", "it", "nl", "ru", "uk", "zh"];

        public static readonly string CultureName = LocalizationBundle.LocaleOverride ?? CultureInfo.CurrentCulture.Name[..2];

        private static readonly bool IsSmallAsian = SmallAsianLanguages.Contains(CultureName);

        public static readonly bool IsAsian = IsSmallAsian || CultureName == "zh";

        private static readonly bool NotEnglish = IsSmallAsian || Locales.Contains(CultureName);

        public static readonly bool IsEnglish = !NotEnglish;

        public static Label CreateLabel(float size, bool applyScale = true)
        {
            return ProcessLabel(new ContreJourDXLabel(size), applyScale);
        }

        public static T ProcessLabel<T>(T label, bool applyScale = true) where T : Label
        {
            float num = 1.4f;
            if (applyScale)
            {
                if (IsSmallAsian)
                {
                    num *= 0.7f;
                }
                else if (NotEnglish)
                {
                    num *= 0.85f;
                }
            }
            float scale = label.Scale * num;
            label.Scale = scale;
            label.Align = TextAlign.Center;
            Vector2 anchor = new(0.5f, 0.5f);
            label.Anchor = anchor;
            label.SetVerticalAnchorToLine(0);
            return label;
        }

        public static Label CreateLabel(float size, string text, bool applyScale = true)
        {
            ContreJourDXLabel contreJourLabel = new(size)
            {
                TextString = text.Localize()
            };
            return ProcessLabel(contreJourLabel, applyScale);
        }

        public static Label CreateMultilineLabel(float size, string text)
        {
            ContreJourDXLabel contreJourLabel = new(size)
            {
                TextString = text.Localize()
            };
            Label label = ProcessLabel(contreJourLabel);
            label.LineSpacing = -6f;
            label.SetVerticalAnchorToLine(0);
            return label;
        }

        public static ProgressLabel CreateProgressLabel(float size, string format, int value, int steps)
        {
            return ProcessLabel(new ProgressLabel(size, format, value, steps));
        }
    }
}
