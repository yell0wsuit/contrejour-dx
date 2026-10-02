using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Mokus2D.Content.Serialization;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Content
{
    public class OneFileResourcesLoader : ResourcesLoader
    {
        private readonly Dictionary<string, IGraphicsDeserializer> _deserializerByType = [];

        // Sprites and movie clips come from the folder's TexturePacker JSON; animations keep their XML.
        private static readonly List<Type> AtlasTypes = [typeof(ISpriteData), typeof(IMovieClipData), typeof(SpriteData), typeof(MovieClipData)];

        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _viewsByFolder = [];

        public OneFileResourcesLoader()
        {
            _deserializerByType["animation"] = Deserializers[typeof(AnimationData)];
        }

        protected override string GetFileName<T>(string resourceName, string resourceSuffix)
        {
            string textureName = GetTextureName(resourceName);
            textureName = ((object)typeof(T) != typeof(AnimationData)) ? (textureName + "sprites" + resourceSuffix) : (textureName + "animations");
            return textureName + ".xml";
        }

        private static string GetTextureName(string resourceName)
        {
            return resourceName[..(resourceName.IndexOf('/') + 1)];
        }

        protected override T LoadData<T>(string name, string resourcesSuffix)
        {
            if (!AtlasTypes.Contains(typeof(T)))
            {
                return base.LoadData<T>(name, resourcesSuffix);
            }
            string folder = name[..name.IndexOf('/')];
            string fileName = folder + "/" + folder + resourcesSuffix + ".json";
            float scaleFactor = resourcesSuffix == null ? 1f : PrefferedScaleFactor;
            List<TextureNodeData> entries;
            using (Stream stream = OpenFile(fileName))
            {
                entries = TexturePackerAtlasReader.Read(stream, folder, scaleFactor, GraphicsRootDirectory, Mokus2DGame.ContentManager.Load, GetViews(folder), fileName);
            }
            // Every entry is announced, as ProcessXml does, so the cache keeps a file's other 2x entries
            // even when the requested one falls back to 1x.
            T val = default;
            foreach (TextureNodeData entry in entries)
            {
                DispatchResourceLoaded(entry.Id, entry);
                if (entry.Id == name)
                {
                    val = (T)(object)entry;
                }
            }
            return val == null ? throw new KeyNotFoundException($"Cannot find resource {name}") : val;
        }

        private Dictionary<string, Dictionary<string, string>> GetViews(string folder)
        {
            if (!_viewsByFolder.TryGetValue(folder, out Dictionary<string, Dictionary<string, string>> views))
            {
                views = ViewConfigReader.Read(GetXml(folder + "/views.xml"));
                _viewsByFolder[folder] = views;
            }
            return views;
        }

        protected override T ProcessXml<T>(string name, XDocument xml)
        {
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
}
