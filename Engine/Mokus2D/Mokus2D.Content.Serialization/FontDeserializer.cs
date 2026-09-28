using System.Xml.Linq;

using Microsoft.Xna.Framework;

using Mokus2D.Fonts;

namespace Mokus2D.Content.Serialization;

public class FontDeserializer(IGraphicsLoader loader) : GraphicsDeserializerBase<FontData>(loader)
{
    public override bool UseSuffix => true;

    public override FontData Deserialize(string id, XElement element)
    {
        string fontName = (string)element.Attribute("name");
        float fontSize = ToSingle((string)element.Attribute("size"));
        float realHeight = ToSingle((string)element.Attribute("realHeight"));
        FontData fontData = new(id, fontName, fontSize, realHeight);
        AddConfig(fontData, element);
        SetScaleFactor(fontData, element);
        SetTexture(element, fontData);
        foreach (XElement item in element.Descendants("symbol"))
        {
            char symbol = item.Attribute("char").Value[0];
            Rectangle rectangle = RectFromXml(item);
            Vector2 anchor = AnchorFromXml(item);
            float width = ToSingle((string)item.Attribute("width"));
            fontData.Add(symbol, rectangle, anchor, width);
        }
        return fontData;
    }
}
