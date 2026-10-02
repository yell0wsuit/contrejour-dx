using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;

namespace Mokus2D.Content.Serialization
{
    public class AnimationDeserializer(IGraphicsLoader loader) : GraphicsDeserializerBase<AnimationData>(loader)
    {
        public override bool UseSuffix => false;

        public override AnimationData Deserialize(string id, XElement element)
        {
            AnimationData animationData = new();
            XAttribute xAttribute = element.Attribute("precalculatedBounds");
            if (xAttribute != null)
            {
                animationData.PrecalculatedBounds = RectFromString(xAttribute.Value);
            }
            AddConfig(animationData, element);
            foreach (XElement item in element.Element("children").Elements())
            {
                Dictionary<string, string> config = GetConfig(item);
                if (config != null)
                {
                    animationData.AddInstanceConfig(item.Name.ToString(), config);
                }
            }
            foreach (XElement item2 in element.Element("frames").Elements())
            {
                animationData.Add(GetAnimationFrame(item2));
            }
            return animationData;
        }

        private List<AnimationFrameData> GetAnimationFrame(XElement element)
        {
            List<AnimationFrameData> list = [];
            foreach (XElement item2 in element.Elements())
            {
                AnimationFrameData animationFrameData = new()
                {
                    Id = item2.Name.ToString(),
                    Alpha = AttributToFloat(item2.Attribute("alpha"), 1f),
                    Position = VectorFromString((string)item2.Attribute("position"), Vector2.Zero),
                    Rotation = AttributToFloat(item2.Attribute("rotation"), 0f),
                    Scale = VectorFromString((string)item2.Attribute("scale"), Vector2.One),
                    Color = AttributToInt(item2.Attribute("color"), 16777215).ToRGBColor(),
                    ColorRatio = AttributToFloat(item2.Attribute("colorRatio"), 0f),
                    Visible = AttributeToBool(item2.Attribute("visible"), defaultValue: true)
                };
                AnimationFrameData item = animationFrameData;
                list.Add(item);
            }
            return list;
        }
    }
}
