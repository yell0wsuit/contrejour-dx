using System.Xml.Linq;

using Mokus2D.Visual.Data;

namespace Mokus2D.Content.Serialization
{
    public abstract class NodeDeserializerBase<T>(IGraphicsLoader loader) : GraphicsDeserializerBase<T>(loader)
    {
        public override bool UseSuffix => true;

        protected void AddConfigAndScaleFactor(TextureNodeData data, XElement element)
        {
            AddConfig(data, element);
            SetScaleFactor(data, element);
        }

        protected FrameData GetFrame(XElement frameXML)
        {
            return new FrameData
            {
                Anchor = AnchorFromXml(frameXML),
                Rect = RectFromXml(frameXML)
            };
        }
    }
}
