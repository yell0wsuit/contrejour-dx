using System.Xml.Linq;

using Mokus2D.Content.Serialization;
using Mokus2D.Fonts;

namespace Mokus2D.Content;

public class ResourcesLoader : ResourcesLoaderBase
{
    private const string FontExtension = "font";

    protected override string GetFileName<T>(string resourceName, string resourcesSuffix)
    {
        string text = (((object)typeof(T) == typeof(FontData)) ? "font" : "xml");
        IGraphicsDeserializer graphicsDeserializer = _deserializers[typeof(T)];
        string text2 = resourceName;
        if (graphicsDeserializer.UseSuffix)
        {
            text2 += resourcesSuffix;
        }
        return text2 + "." + text;
    }

    protected override T ProcessXml<T>(string name, XDocument xml)
    {
        IGraphicsDeserializer graphicsDeserializer = _deserializers[typeof(T)];
        object obj = graphicsDeserializer.Deserialize(name, xml.Root);
        DispatchResourceLoaded(name, obj);
        return (T)obj;
    }
}
