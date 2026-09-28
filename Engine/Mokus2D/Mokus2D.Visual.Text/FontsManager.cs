using System;
using System.Collections.Generic;
using Mokus2D.Fonts;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Text;

public class FontsManager
{
	private readonly Dictionary<string, SortedList<FontData>> _fonts = new Dictionary<string, SortedList<FontData>>();

	public FontData GetFontData(string fontName, float size)
	{
		SortedList<FontData> sortedList = _fonts.TryGetValue(fontName) ?? _fonts.TryGetValue(RemoveSpaces(fontName));
		if (sortedList == null)
		{
			throw new Exception("Font not found, try calling FontClass.Register() static method in application OnInitialize()");
		}
		foreach (FontData item in sortedList)
		{
			if (item.FontSize >= size)
			{
				return item;
			}
		}
		return sortedList.Last();
	}

	private static string RemoveSpaces(string fontName)
	{
		return fontName.Replace(" ", "");
	}

	public void ReloadFonts()
	{
		foreach (SortedList<FontData> value in _fonts.Values)
		{
			List<FontData> list = new List<FontData>(value);
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
		if (!_fonts.ContainsKey(fontName))
		{
			_fonts[fontName] = new SortedList<FontData>(FontsComparizon);
		}
		SortedList<FontData> sortedList = _fonts[fontName];
		sortedList.Add(Mokus2DGame.LoadResource<FontData>(fontId));
	}

	private int FontsComparizon(FontData a, FontData b)
	{
		return Comparisons.FloatComparizon(a.FontSize, b.FontSize);
	}
}
