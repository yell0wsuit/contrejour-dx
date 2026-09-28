using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Mokus2D.Content.Serialization;
using Mokus2D.Fonts;
using Mokus2D.Util;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Particles.Data;

namespace Mokus2D.Content;

public abstract class ResourcesLoaderBase : IGraphicsLoader
{
    protected const string XmlExtension = "xml";

    protected const string ExtensionSeparator = ".";

    protected readonly Dictionary<Type, IGraphicsDeserializer> _deserializers = [];

    private string _resourcesSuffix;

    private float _prefferedScaleFactor = 1f;

    public string GraphicsRootDirectory { get; set; }

    public bool IsAbsolutePath { get; set; }

    public bool FallbackToDefaultScaleFactor { get; set; }

    public float PrefferedScaleFactor
    {
        get => _prefferedScaleFactor;
        set
        {
            if (_prefferedScaleFactor != value)
            {
                _prefferedScaleFactor = value;
                _resourcesSuffix = value != 1f ? ContentUtil.GetResourcesSuffix(value) : null;
            }
        }
    }

    public event Action<string, object> ResourceLoaded;

    protected ResourcesLoaderBase()
    {
        _deserializers[typeof(ISpriteData)] = new SpriteDeserializer(this);
        _deserializers[typeof(SpriteData)] = _deserializers[typeof(ISpriteData)];
        _deserializers[typeof(IMovieClipData)] = new MovieClipDeserializer(this);
        _deserializers[typeof(MovieClipData)] = _deserializers[typeof(IMovieClipData)];
        _deserializers[typeof(AnimationData)] = new AnimationDeserializer(this);
        _deserializers[typeof(FontData)] = new FontDeserializer(this);
        _deserializers[typeof(ParticleSystemConfig)] = new ParicleConfigDeserializer();
    }

    public void Unload(string name)
    {
        throw new NotImplementedException();
    }

    protected abstract string GetFileName<T>(string resourceName, string resourceSuffix = null);

    protected abstract T ProcessXml<T>(string name, XDocument xml);

    public T Load<T>(string name)
    {
        try
        {
            return LoadData<T>(name, _resourcesSuffix);
        }
        catch (Exception)
        {
            if (FallbackToDefaultScaleFactor && _resourcesSuffix != null)
            {
                return LoadData<T>(name, null);
            }
            throw;
        }
    }

    private T LoadData<T>(string name, string resourcesSuffix)
    {
        string fileName = GetFileName<T>(name, resourcesSuffix);
        XDocument xml = GetXml(fileName);
        return ProcessXml<T>(name, xml);
    }

    protected void DispatchResourceLoaded(string name, object data)
    {
        ResourceLoaded.Dispatch(name, data);
    }

    private XDocument GetXml(string name)
    {
        string fullPath = GetFullPath(name);
        try
        {
            using Stream stream = Mokus2DGame.FileLoader.OpenFile(fullPath);
            using StreamReader textReader = new(stream);
            return XDocument.Load(textReader);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    private string GetFullPath(string name)
    {
        return IsAbsolutePath
            ? PathUtil.Combine(GraphicsRootDirectory, name)
            : PathUtil.Combine(Mokus2DGame.ContentManager.RootDirectory, GraphicsRootDirectory, name);
    }
}
