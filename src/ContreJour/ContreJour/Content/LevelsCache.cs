using System.Collections.Generic;
using System.IO;
using System.Reflection;

using ContreJour.Config;
using ContreJour.Gameplay;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Content;
using Mokus2D.Util.Xml;

namespace ContreJour.Content
{
    public class LevelsCache
    {
        private readonly string clip_root = "Levels/";

        private readonly XmlSerializer serializer = new(typeof(ContreJourApplication).GetTypeInfo().Assembly);
        private readonly MokusContentManager content;

        public Dictionary<string, Level> CachedLevels { get; } = [];

        private static string CorrectName(string name)
        {
            string[] array = name.Split('/', '\\');
            return array[^1];
        }

        public LevelsCache()
        {
            serializer.AddAlias("Level", typeof(Level).AssemblyQualifiedName);
            serializer.AddAlias("NSMutableArray", typeof(List<object>).FullName);
            serializer.AddAlias("NSMutableDictionary", typeof(Hashtable).FullName);
            serializer.AddAlias("FlashPoint", typeof(Vector2).AssemblyQualifiedName);
            serializer.AddAlias("x", "X");
            serializer.AddAlias("y", "Y");
            serializer.AddAlias("width", "Width");
            serializer.AddAlias("height", "Height");
            serializer.AddAlias("frames", "Frames");
            serializer.AddAlias("tileData", "TileData");
            serializer.AddAlias("anchor", "Anchor");
            serializer.AddAlias("useSheet", "UseSheet");
            content = Mokus2DGame.ContentManager;
        }

        public Level Load(string name)
        {
            name = CorrectName(name);
            if (!CachedLevels.TryGetValue(name, out Level level))
            {
                string path = Path.ChangeExtension(Path.Combine(
                [
                    Path.Combine([content.RootDirectory, clip_root]),
                    Path.Combine(
                    [
                        ContreJourConfig.AspectRatio.LevelsFolder,
                        name
                    ])
                ]), "xml");
                Stream stream = Mokus2DGame.FileLoader.OpenFile(path);
                level = (Level)serializer.DeserializeFile(stream);
                CachedLevels[name] = level;
            }
            return level;
        }
    }
}
