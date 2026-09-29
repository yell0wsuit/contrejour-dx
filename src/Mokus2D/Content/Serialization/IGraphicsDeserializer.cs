using System.Xml.Linq;

namespace Mokus2D.Content.Serialization
{
    public interface IGraphicsDeserializer
    {
        bool UseSuffix { get; }

        object Deserialize(string id, XElement document);
    }
    public interface IGraphicsDeserializer<out T> : IGraphicsDeserializer
    {
        new T Deserialize(string id, XElement element);
    }
}
