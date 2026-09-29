using System.Linq;
using System.Xml.Linq;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Content.Serialization
{
    public class SpriteDeserializer(IGraphicsLoader loader) : NodeDeserializerBase<ISpriteData>(loader)
    {
        public override ISpriteData Deserialize(string id, XElement element)
        {
            SpriteData spriteData = new(id);
            AddConfigAndScaleFactor(spriteData, element);
            SetTexture(element, spriteData);
            spriteData.Frame = GetFrame(element.Descendants("frame").First());
            return spriteData;
        }
    }
}
