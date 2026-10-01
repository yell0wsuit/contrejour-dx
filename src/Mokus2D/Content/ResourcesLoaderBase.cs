using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;

using Mokus2D.Content.Serialization;
using Mokus2D.Util;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Particles.Data;

namespace Mokus2D.Content
{
    public abstract class ResourcesLoaderBase : IGraphicsLoader
    {
        protected const string XmlExtension = "xml";

        protected const string ExtensionSeparator = ".";

        protected Dictionary<Type, IGraphicsDeserializer> Deserializers { get; } = [];

        private string _resourcesSuffix;

        public string GraphicsRootDirectory { get; set; }

        public bool IsAbsolutePath { get; set; }

        public bool FallbackToDefaultScaleFactor { get; set; }

        public float PrefferedScaleFactor
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    _resourcesSuffix = value != 1f ? ContentUtil.GetResourcesSuffix(value) : null;
                }
            }
        } = 1f;

        public event Action<string, object> ResourceLoaded;

        protected ResourcesLoaderBase()
        {
            Deserializers[typeof(AnimationData)] = new AnimationDeserializer(this);
            Deserializers[typeof(ParticleSystemConfig)] = new ParicleConfigDeserializer();
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
            // A missing file or name falls back to the 1x art; broken atlas data must surface instead of
            // silently loading the 1x version.
            catch (Exception exception) when (exception is not InvalidDataException and not JsonException)
            {
                if (FallbackToDefaultScaleFactor && _resourcesSuffix != null)
                {
                    return LoadData<T>(name, null);
                }
                throw;
            }
        }

        protected virtual T LoadData<T>(string name, string resourcesSuffix)
        {
            string fileName = GetFileName<T>(name, resourcesSuffix);
            XDocument xml = GetXml(fileName);
            return ProcessXml<T>(name, xml);
        }

        protected void DispatchResourceLoaded(string name, object data)
        {
            ResourceLoaded.Dispatch(name, data);
        }

        protected XDocument GetXml(string name)
        {
            try
            {
                using Stream stream = OpenFile(name);
                using StreamReader textReader = new(stream);
                return XDocument.Load(textReader);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        protected Stream OpenFile(string name)
        {
            return Mokus2DGame.FileLoader.OpenFile(GetFullPath(name));
        }

        private string GetFullPath(string name)
        {
            return IsAbsolutePath
                ? PathUtil.Combine(GraphicsRootDirectory, name)
                : PathUtil.Combine(Mokus2DGame.ContentManager.RootDirectory, GraphicsRootDirectory, name);
        }
    }
}
