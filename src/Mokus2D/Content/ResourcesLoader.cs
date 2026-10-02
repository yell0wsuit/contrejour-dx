using System.Xml.Linq;

using Mokus2D.Content.Serialization;

namespace Mokus2D.Content
{
    public class ResourcesLoader : ResourcesLoaderBase
    {
        protected override string GetFileName<T>(string resourceName, string resourceSuffix)
        {
            string text = "xml";
            IGraphicsDeserializer graphicsDeserializer = Deserializers[typeof(T)];
            string text2 = resourceName;
            if (graphicsDeserializer.UseSuffix)
            {
                text2 += resourceSuffix;
            }
            return text2 + "." + text;
        }

        protected override T ProcessXml<T>(string name, XDocument xml)
        {
            IGraphicsDeserializer graphicsDeserializer = Deserializers[typeof(T)];
            object obj = graphicsDeserializer.Deserialize(name, xml.Root);
            DispatchResourceLoaded(name, obj);
            return (T)obj;
        }
    }
}
