using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Fonts;

public class FontData : TextureNodeData
{
    private const char SpecialSymbolsStart = '\ue000';

    public readonly string FontName;

    public readonly float FontSize;

    public readonly float RealHeight;

    private readonly Dictionary<char, CharData> _chars = new Dictionary<char, CharData>();

    public CharData this[char key] => _chars.TryGetValue(key);

    public static char GetSpecialSymbol(int index)
    {
        return (char)(57344 + index);
    }

    public FontData(string id, string fontName, float fontSize, float realHeight)
        : base(id)
    {
        FontName = fontName;
        FontSize = fontSize;
        RealHeight = realHeight;
    }

    public void AddSpecialSymbol(char symbol, Rectangle rectangle, Vector2 anchorInPixels, float width)
    {
        Add(symbol, rectangle, anchorInPixels, width);
    }

    public void AddSpecialSpriteSymbol(char symbol, Rectangle rectangle, Vector2 relativeAnchor)
    {
        AddSpecialSymbol(symbol, rectangle, relativeAnchor * rectangle.Size(), rectangle.Width);
    }

    public void AddSpecialSpriteSymbol(char symbol, ISpriteData data)
    {
        AddSpecialSpriteSymbol(symbol, data.TextureRect, data.Anchor);
    }

    public void AddSpecialSpriteSymbol(char symbol, string id)
    {
        ISpriteData data = Mokus2DGame.LoadSpriteData(id);
        AddSpecialSpriteSymbol(symbol, data);
    }

    public void Add(char symbol, Rectangle rectangle, Vector2 anchor, float width)
    {
        _chars[symbol] = new CharData(this, rectangle, anchor, width);
    }
}
