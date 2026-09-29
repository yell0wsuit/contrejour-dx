using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Microsoft.Xna.Framework.Graphics;

using Mokus2D.FileSystem;

namespace Mokus2D.Content
{
    // Caches textures decoded from the PNG files under RootDirectory. Texture2D and
    // Texture2D.FromStream are still MonoGame's: textures do not go through a platform interface yet.
    public sealed class MokusContentManager(IFileLoader files) : IDisposable
    {
        private readonly Dictionary<string, Texture2D> _loadedAssets = [];

        // Kept after Unload so a disposed texture can still be named in error messages.
        private readonly Dictionary<Texture2D, string> _loadedTextures = [];

        public string RootDirectory { get; set; } = string.Empty;

        public Texture2D Load(string assetName)
        {
            if (_loadedAssets.TryGetValue(assetName, out Texture2D texture))
            {
                return texture;
            }
            try
            {
                texture = ReadTexture(assetName);
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

        public string GetDisposedTextureName(Texture2D texture)
        {
            return _loadedTextures[texture];
        }

        public void Unload()
        {
            foreach (Texture2D texture in _loadedAssets.Values)
            {
                texture.Dispose();
            }
            _loadedAssets.Clear();
        }

        public void Dispose()
        {
            Unload();
        }

        private Texture2D ReadTexture(string assetName)
        {
            string path = Path.Combine(RootDirectory, Path.ChangeExtension(assetName, ".png"));
            using Stream stream = files.OpenFile(path);
            // Sprites are drawn with premultiplied-alpha blending, as MonoGame's content loader
            // prepared raw image files; straight alpha shows white fringes around soft edges.
            return Texture2D.FromStream(Mokus2DGame.Device, stream, DefaultColorProcessors.PremultiplyAlpha);
        }
    }
}
