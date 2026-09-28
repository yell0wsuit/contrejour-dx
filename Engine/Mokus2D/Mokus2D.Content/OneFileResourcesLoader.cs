using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Mokus2D.Content.Serialization;
using Mokus2D.Fonts;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Particles.Data;

namespace Mokus2D.Content;

public class OneFileResourcesLoader : ResourcesLoader
{
    private const string Animations = "animations";

    private const string Sprites = "sprites";

    private static readonly List<Type> SeparateFileTypes = [typeof(ParticleSystemConfig)];

    private readonly Dictionary<string, IGraphicsDeserializer> _deserializerByType = [];

    public OneFileResourcesLoader()
    {
        _deserializerByType["animation"] = _deserializers[typeof(AnimationData)];
        _deserializerByType["sprite"] = _deserializers[typeof(SpriteData)];
        _deserializerByType["movieClip"] = _deserializers[typeof(MovieClipData)];
        _deserializerByType["font"] = _deserializers[typeof(FontData)];
    }

    protected override string GetFileName<T>(string resourceName, string resourceSuffix)
    {
        if (SeparateFileTypes.Contains(typeof(T)))
        {
            return base.GetFileName<T>(resourceName, resourceSuffix);
        }
        string textureName = GetTextureName(resourceName);
        textureName = ((object)typeof(T) != typeof(AnimationData)) ? (textureName + "sprites" + resourceSuffix) : (textureName + "animations");
        return textureName + ".xml";
    }

    private static string GetTextureName(string resourceName)
    {
        return resourceName[..(resourceName.IndexOf('/') + 1)];
    }

    protected override T ProcessXml<T>(string name, XDocument xml)
    {
        if (SeparateFileTypes.Contains(typeof(T)))
        {
            return base.ProcessXml<T>(name, xml);
        }
        T val = default;
        string textureName = GetTextureName(name);
        foreach (XElement item in xml.Root.Elements())
        {
            string value = item.Attribute("type").Value;
            IGraphicsDeserializer graphicsDeserializer = _deserializerByType[value];
            string text = textureName + item.Name;
            object obj = graphicsDeserializer.Deserialize(text, item);
            DispatchResourceLoaded(text, obj);
            if (text == name)
            {
                val = (T)obj;
            }
        }
        return val == null ? throw new KeyNotFoundException($"Cannot find resource {name}") : val;
    }
}
