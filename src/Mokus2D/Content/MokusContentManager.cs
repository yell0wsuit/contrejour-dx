using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util;

namespace Mokus2D.Content
{
    public class MokusContentManager(IServiceProvider serviceProvider) : ContentManager(serviceProvider)
    {
        private static readonly string[] TextureExtensions = [".png"];

        private readonly Dictionary<string, object> _loadedAssets = [];

        private readonly Dictionary<Texture2D, string> _loadedTextures = [];

        public override T Load<T>(string assetName)
        {
            T val = (T)_loadedAssets.GetValueOrDefault(assetName);
            if (val != null)
            {
                return val;
            }
            try
            {
                val = ReadAsset<T>(assetName);
            }
            catch (OutOfMemoryException innerException)
            {
                throw new InsufficientMemoryException(string.Format(CultureInfo.InvariantCulture, "Out of memory while loading {0}", assetName), innerException);
            }
            if (val is Texture2D texture2D)
            {
                texture2D.Name = assetName;
                _loadedTextures.Add(texture2D, assetName);
            }
            _loadedAssets[assetName] = val;
            return val;
        }

        public string GetDisposedTextureName(Texture2D texture)
        {
            return _loadedTextures[texture];
        }

        protected Texture2D ReadTextureAsset(string assetName)
        {
            Texture2D result;
            try
            {
                result = ReadAsset<Texture2D>(assetName, null);
            }
            catch (ContentLoadException exception)
            {
                using Stream stream = GetTextureStream(assetName);
                if (stream != null)
                {
                    result = Texture2D.FromStream(Mokus2DGame.Device, stream);
                }
                else
                {
                    result = null;
                    ExceptionUtil.Throw(exception);
                }
            }
            return result;
        }

        private Stream GetTextureStream(string assetName)
        {
            string[] textureExtensions = TextureExtensions;
            foreach (string extension in textureExtensions)
            {
                string path = Path.Combine(
                [
                    RootDirectory,
                    Path.ChangeExtension(assetName, extension)
                ]);
                try
                {
                    return Mokus2DGame.FileLoader.OpenFile(path);
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        protected virtual T ReadAsset<T>(string assetName)
        {
            return (object)typeof(T) == typeof(Texture2D) ? (T)(object)ReadTextureAsset(assetName) : ReadAsset<T>(assetName, null);
        }

        public override void Unload()
        {
            base.Unload();
            foreach (KeyValuePair<string, object> loadedAsset in _loadedAssets)
            {
                if (loadedAsset.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _loadedAssets.Clear();
        }
    }
}
