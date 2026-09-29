using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D;
using Mokus2D.Content;
using Mokus2D.Util.Xml;
using Mokus2D.Visual.Data;

namespace Default.Namespace;

public class ClipFactory
{
    private static readonly List<TextureSource> textureSources = [];

    private static TextureSource defaultTextureSource = new("mc/hd/", 1f, 640);

    private static TextureSource currentTextureSource;

    private static readonly XmlSerializer serializer = new();

    public static readonly MokusContentManager content;

    private static readonly Dictionary<string, ClipData> configsCache = [];

    public static ClipFactory Instance { get; } = new();

    public static Dictionary<string, string> FullPaths { get; } = [];

    static ClipFactory()
    {
        textureSources.Add(defaultTextureSource);
        textureSources.Add(new TextureSource("mc/shd/", 0.5f, 1300));
        ChooseTextureSource();
        serializer.AddAlias("NSMutableDictionary", typeof(ClipData).AssemblyQualifiedName);
        serializer.AddAlias("FlashPoint", typeof(Vector2).AssemblyQualifiedName);
        serializer.AddAlias("x", "X");
        serializer.AddAlias("y", "Y");
        serializer.AddAlias("useSheet", "UseSheet");
        serializer.AddAlias("width", "Width");
        serializer.AddAlias("height", "Height");
        serializer.AddAlias("frames", "FramesCount");
        serializer.AddAlias("tileData", "TileData");
        serializer.AddAlias("anchor", "Anchor");
        serializer.AddAlias("jpg", "Jpg");
        content = Mokus2DGame.ContentManager;
    }

    private static void ChooseTextureSource()
    {
        int i;
        for (i = 0; i < textureSources.Count; i++)
        {
            TextureSource textureSource = textureSources[i];
            if (Mokus2DGame.Device.Viewport.Width < textureSource.NeededWidth)
            {
                break;
            }
        }
        currentTextureSource = textureSources[i - 1];
        if (currentTextureSource == defaultTextureSource)
        {
            defaultTextureSource = textureSources.Last();
        }
    }

    private static string CorrectName(string name)
    {
        name = name.Split('/', '\\').Last().Split('.')
            .First();
        return name;
    }

    public static Texture2D GetTexture(string name)
    {
        return content.Load<Texture2D>($"Graphics/textures/{name}");
    }

    public static string GetTexturePath(string name)
    {
        name = CorrectName(name);
        return File.Exists(GetPath(name, currentTextureSource, "xnb"))
            ? Path.Combine([currentTextureSource.Path, name])
            : Path.Combine([defaultTextureSource.Path, name]);
    }

    private static ClipData GetConfigByName(string name)
    {
        name = CorrectName(name);
        if (!configsCache.TryGetValue(name, out ClipData clipData))
        {
            clipData = TryReadConfig(name, currentTextureSource) ?? TryReadConfig(name, defaultTextureSource);
            configsCache[name] = clipData;
        }
        return clipData;
    }

    private static ClipData TryReadConfig(string name, TextureSource textureSource)
    {
        try
        {
            ClipData clipData = ReadConfig(name, textureSource);
            clipData.ScaleFactor = textureSource.TextureScaleFactor;
            return clipData;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static ClipData ReadConfig(string name, TextureSource textureSource)
    {
        string path = GetPath(name, textureSource, "xml");
        using Stream stream = Mokus2DGame.FileLoader.OpenFile(path);
        ClipData clipData = (ClipData)serializer.DeserializeFile(stream);
        clipData.Initialize();
        configsCache[name] = clipData;
        return clipData;
    }

    private static string GetPath(string name, TextureSource textureSource)
    {
        return Path.Combine(
        [
            content.RootDirectory,
            textureSource.Path + name
        ]);
    }

    private static string GetPath(string name, TextureSource textureSource, string extension)
    {
        return Path.ChangeExtension(GetPath(name, textureSource), extension);
    }

    public static bool HasConfig(string name)
    {
        return GetConfigByName(name) != null;
    }
}
