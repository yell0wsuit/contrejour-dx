using System.Xml.Linq;
using System.Xml.Serialization;

using Mokus2D.Visual.Particles.Data;

namespace Mokus2D.Content.Serialization;

public class ParicleConfigDeserializer : IGraphicsDeserializer<ParticleSystemConfig>, IGraphicsDeserializer
{
    private readonly XmlSerializer _xmlSerializer = new(typeof(ParticleSystemConfig));

    public bool UseSuffix => false;

    object IGraphicsDeserializer.Deserialize(string id, XElement document)
    {
        return Deserialize(id, document);
    }

    public ParticleSystemConfig Deserialize(string id, XElement element)
    {
        return (ParticleSystemConfig)_xmlSerializer.Deserialize(element.CreateReader());
    }
}
