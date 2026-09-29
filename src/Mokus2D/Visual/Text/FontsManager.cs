using System;
using System.Collections.Generic;

using Mokus2D.Fonts;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Text
{
    public class FontsManager
    {
        private readonly Dictionary<string, SortedCollection<FontData>> _fonts = [];

        public FontData GetFontData(string fontName, float size)
        {
            SortedCollection<FontData> sortedList = (_fonts.GetValueOrDefault(fontName) ?? _fonts.GetValueOrDefault(RemoveSpaces(fontName))) ?? throw new InvalidOperationException("Font not found, try calling FontClass.Register() static method in application OnInitialize()");
            foreach (FontData item in sortedList)
            {
                if (item.FontSize >= size)
                {
                    return item;
                }
            }
            return sortedList[^1];
        }

        private static string RemoveSpaces(string fontName)
        {
            return fontName.Replace(" ", "");
        }

        public void ReloadFonts()
        {
            foreach (SortedCollection<FontData> value in _fonts.Values)
            {
                List<FontData> list = [.. value];
                value.Clear();
                foreach (FontData item2 in list)
                {
                    FontData item = Mokus2DGame.LoadResource<FontData>(item2.Id);
                    value.Add(item);
                }
            }
        }

        public void RegisterFont(string fontName, string fontId)
        {
            if (!_fonts.TryGetValue(fontName, out SortedCollection<FontData> sortedList))
            {
                sortedList = new SortedCollection<FontData>(FontsComparizon);
                _fonts[fontName] = sortedList;
            }
            sortedList.Add(Mokus2DGame.LoadResource<FontData>(fontId));
        }

        private int FontsComparizon(FontData a, FontData b)
        {
            return Comparisons.FloatComparizon(a.FontSize, b.FontSize);
        }
    }
}
