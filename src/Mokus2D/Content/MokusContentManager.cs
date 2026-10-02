using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Mokus2D.Diagnostics;
using Mokus2D.FileSystem;
using Mokus2D.Graphics;

namespace Mokus2D.Content
{
    // Caches textures decoded from the image files under RootDirectory.
    public sealed class MokusContentManager(IFileLoader files, IRenderer renderer, ContentFormats formats) : IDisposable
    {
        private readonly Dictionary<string, ITexture> _loadedAssets = [];

        // Kept after Unload so a disposed texture can still be named in error messages.
        private readonly Dictionary<ITexture, string> _loadedTextures = [];

        public string RootDirectory { get; set; } = string.Empty;

        public ITexture Load(string assetName)
        {
            if (_loadedAssets.TryGetValue(assetName, out ITexture texture))
            {
                return texture;
            }
            try
            {
                try
                {
                    texture = ReadTexture(assetName);
                }
                catch (Exception failure)
                {
                    EngineLog.TextureFailed(Log.For(LogCategories.Content), assetName, failure);
                    throw;
                }
            }
            catch (OutOfMemoryException innerException)
            {
                throw new InsufficientMemoryException(string.Format(CultureInfo.InvariantCulture, "Out of memory while loading {0}", assetName), innerException);
            }
            texture.Name = assetName;
            _loadedTextures.Add(texture, assetName);
            _loadedAssets[assetName] = texture;
            return texture;
        }

        public string GetDisposedTextureName(ITexture texture)
        {
            return _loadedTextures[texture];
        }

        public void Unload()
        {
            foreach (ITexture texture in _loadedAssets.Values)
            {
                texture.Dispose();
            }
            _loadedAssets.Clear();
        }

        public void Dispose()
        {
            Unload();
        }

        private ITexture ReadTexture(string assetName)
        {
            string path = Path.Combine(RootDirectory, Path.ChangeExtension(assetName, formats.Image));
            using Stream stream = files.OpenFile(path);
            return renderer.CreateTexture(stream);
        }
    }
}
