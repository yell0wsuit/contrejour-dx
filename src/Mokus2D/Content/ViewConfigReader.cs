using System.Collections.Generic;
using System.Xml.Linq;

namespace Mokus2D.Content
{
    // Reads a folder's views.xml: per-sprite <config> scene data (layer, z, speed, viewType...) that
    // TexturePacker cannot carry, so it lives beside the atlas and survives a repack.
    public static class ViewConfigReader
    {
        public static Dictionary<string, Dictionary<string, string>> Read(XDocument views)
        {
            Dictionary<string, Dictionary<string, string>> result = [];
            if (views == null)
            {
                return result;
            }
            foreach (XElement view in views.Root.Elements())
            {
                XElement config = view.Element("config");
                if (config == null)
                {
                    continue;
                }
                // Same keys and order GraphicsDeserializerBase.GetConfig produced from the old sprites.xml.
                Dictionary<string, string> values = [];
                foreach (XAttribute attribute in config.Attributes())
                {
                    values[attribute.Name.ToString()] = attribute.Value;
                }
                result[view.Name.LocalName] = values;
            }
            return result;
        }
    }
}
