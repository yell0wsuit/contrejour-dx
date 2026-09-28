using System;
using System.Collections.Generic;
using System.Globalization;

using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D.Fonts;
using Mokus2D.Visual.Text;

namespace ContreJour.Utils;

public static class ContreJourLabelUtil
{
    private static readonly List<string> SmallAsianLanguages = new List<string>(new string[2] { "ja", "ko" });

    private static readonly List<string> Locales = new List<string>(new string[8] { "de", "es", "fr", "it", "nl", "ru", "uk", "zh" });

    public static readonly string CultureName = CultureInfo.CurrentCulture.Name.Substring(0, 2);

    private static readonly bool IsSmallAsian = SmallAsianLanguages.Contains(CultureName);

    public static readonly bool IsAsian = IsSmallAsian || CultureName == "zh";

    private static readonly bool NotEnglish = IsSmallAsian || Locales.Contains(CultureName);

    public static readonly bool IsEnglish = !NotEnglish;

    public static Label CreateLabel(float size, bool applyScale = true)
    {
        return ProcessLabel(new ContreJourLabel(size), applyScale);
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
        Vector2 anchor = new Vector2(0.5f, 0.5f);
        label.Anchor = anchor;
        label.SetVerticalAnchorToLine(0);
        return label;
    }

    public static Label CreateLabel(float size, string text, bool applyScale = true)
    {
        ContreJourLabel contreJourLabel = new ContreJourLabel(size);
        contreJourLabel.TextString = text.Localize();
        return ProcessLabel(contreJourLabel, applyScale);
    }

    public static Label CreateMultilineLabel(float size, string text)
    {
        ContreJourLabel contreJourLabel = new ContreJourLabel(size);
        contreJourLabel.TextString = text.Localize();
        Label label = ProcessLabel(contreJourLabel);
        label.LineSpacing = -6f;
        label.SetVerticalAnchorToLine(0);
        return label;
    }

    public static ProgressLabel CreateProgressLabel(float size, string format, int value, int steps)
    {
        return ProcessLabel(new ProgressLabel(size, format, value, steps));
    }

    private static FontData GetFont(float size, out float scale)
    {
        FontData fontData = null;
        scale = 1f;
        int num = 0;
        foreach (KeyValuePair<int, FontData> font in ContreJourApplication.Fonts)
        {
            num = Math.Max(num, font.Key);
            if ((float)font.Key >= size)
            {
                fontData = font.Value;
                scale *= size / (float)font.Key;
                break;
            }
        }
        if (fontData == null)
        {
            fontData = ContreJourApplication.Fonts[num];
            scale *= size / (float)num;
        }
        return fontData;
    }
}
