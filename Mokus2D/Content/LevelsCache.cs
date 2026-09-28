using System.Collections.Generic;
using System.IO;
using System.Reflection;

using ContreJour;
using ContreJour.Config;

using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Xml;

namespace Mokus2D.Content;

public class LevelsCache
{
    private readonly string clip_root = "Levels/";

    private XmlSerializer serializer = new XmlSerializer(typeof(ContreJourApplication).GetTypeInfo().Assembly);

    private Dictionary<string, Level> cache = new Dictionary<string, Level>();

    private MokusContentManager content;

    public Dictionary<string, Level> CachedLevels => cache;

    private string correctName(string name)
    {
        string[] array = name.Split('/', '\\');
        return array[array.Length - 1];
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
        name = correctName(name);
        Level level;
        if (!cache.ContainsKey(name))
        {
            string path = Path.ChangeExtension(Path.Combine(new string[2]
            {
                Path.Combine(new string[2] { content.RootDirectory, clip_root }),
                Path.Combine(new string[2]
                {
                    ContreJourConfig.AspectRatio.LevelsFolder,
                    name
                })
            }), "xml");
            Stream stream = Mokus2DGame.FileLoader.OpenFile(path);
            level = (Level)serializer.DeserializeFile(stream);
            cache[name] = level;
        }
        else
        {
            level = cache[name];
        }
        return level;
    }
}
