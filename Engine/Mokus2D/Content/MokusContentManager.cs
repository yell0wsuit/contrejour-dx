using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util;

namespace Mokus2D.Content;

public class MokusContentManager(IServiceProvider serviceProvider) : ContentManager(serviceProvider)
{
    public const bool DebugDisposedTextures = true;

    private static readonly CompositeFormat TextureNameFormat = CompositeFormat.Parse("{0}/{0}{1}");

    private static readonly string[] TextureExtensions = [".png"];

    private readonly Dictionary<string, object> _loadedAssets = [];

    private readonly Dictionary<Texture2D, string> _loadedTextures = [];

    public bool IsLoaded(string assetName)
    {
        return _loadedAssets.ContainsKey(assetName);
    }

    public static string GetFullTexturePath(string name)
    {
        return GetFullTexturePath(name, Mokus2DGame.Config.GraphicsLoader.PrefferedScaleFactor);
    }

    public static string GetFullTexturePath(string name, float scaleFactor)
    {
        string text = string.Format(CultureInfo.InvariantCulture, TextureNameFormat, name, ContentUtil.GetResourcesSuffix(scaleFactor));
        return Path.Combine(
        [
            Mokus2DGame.Config.GraphicsLoader.GraphicsRootDirectory,
            text
        ]);
    }

    public string FindAssetName(object asset)
    {
        foreach (KeyValuePair<string, object> loadedAsset in _loadedAssets)
        {
            if (loadedAsset.Value == asset)
            {
                return loadedAsset.Key;
            }
        }
        return null;
    }

    public override T Load<T>(string assetName)
    {
        T val = (T)_loadedAssets.TryGetValue(assetName);
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
            throw new InsufficientMemoryException("Out of memory while loading {0}".FormatThis(assetName), innerException);
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

    public void UnloadTexture(string shortName, float scaleFactor)
    {
        UnloadTexture(GetFullTexturePath(shortName, scaleFactor));
    }

    public void UnloadTexture(string fullName)
    {
        object obj = _loadedAssets[fullName];
        _ = _loadedAssets.Remove(fullName);
        ((Texture2D)obj).Dispose();
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

    public List<string> GetLoadedTextures()
    {
        return [.. from asset in _loadedAssets
                where asset.Value is Texture2D
                select asset.Key];
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
