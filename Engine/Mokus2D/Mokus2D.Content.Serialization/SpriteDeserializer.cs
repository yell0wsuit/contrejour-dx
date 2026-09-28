using System.Linq;
using System.Xml.Linq;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Content.Serialization;

public class SpriteDeserializer : NodeDeserializerBase<ISpriteData>
{
    public SpriteDeserializer(IGraphicsLoader loader)
        : base(loader)
    {
    }

    public override ISpriteData Deserialize(string id, XElement element)
    {
        SpriteData spriteData = new SpriteData(id);
        AddConfigAndScaleFactor(spriteData, element);
        SetTexture(element, spriteData);
        spriteData.Frame = GetFrame(element.Descendants("frame").First());
        return spriteData;
    }
}
