using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

using Mokus2D;
using Mokus2D.Content;
using Mokus2D.Graphics;
using Mokus2D.Util.Xml;
using Mokus2D.Visual.Data;

namespace ContreJour.Gameplay
{
    public class ClipFactory
    {
        private static readonly List<TextureSource> textureSources = [];

        private static TextureSource defaultTextureSource = new("mc/hd/", 1f, 640);

        private static TextureSource currentTextureSource;

        private static readonly XmlSerializer serializer = new();

        public static readonly MokusContentManager content;

        private static readonly Dictionary<string, ClipData> configsCache = [];

        // Scaled texture names already known to be missing, so each lookup throws at most once.
        private static readonly HashSet<string> missingScaledTextures = [];

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
                if (Mokus2DGame.Instance.WindowSize.X < textureSource.NeededWidth)
                {
                    break;
                }
            }
            currentTextureSource = textureSources[i - 1];
            if (currentTextureSource == defaultTextureSource)
            {
                defaultTextureSource = textureSources[^1];
            }
        }

        private static string CorrectName(string name)
        {
            name = name.Split('/', '\\')[^1].Split('.')[0];
            return name;
        }

        // Windows 8 shipped 2x copies of some repeating textures as "<name>.x0.5.png" but never loaded them.
        // Texture coordinates are fractions of the texture, so the larger copy maps exactly like the original.
        public static ITexture GetTexture(string name)
        {
            string asset = $"Graphics/textures/{name}";
            float scaleFactor = Mokus2DGame.Config.GraphicsLoader.PrefferedScaleFactor;
            if (scaleFactor != 1f)
            {
                // The explicit extension keeps the content manager's ChangeExtension from eating the ".5".
                string scaled = $"{asset}.x{scaleFactor.ToString(CultureInfo.InvariantCulture)}.png";
                if (!missingScaledTextures.Contains(scaled))
                {
                    try
                    {
                        return content.Load(scaled);
                    }
                    catch (FileNotFoundException)
                    {
                        _ = missingScaledTextures.Add(scaled);
                    }
                }
            }
            return content.Load(asset);
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
}
