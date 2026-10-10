using System.Collections.Generic;
using System.IO;

using ContreJourDX.Config;
using ContreJourDX.Gameplay;

using Mokus2D;
using Mokus2D.Content;

namespace ContreJourDX.Content
{
    public class LevelsCache
    {
        private readonly string clip_root = "Levels/";

        private readonly MokusContentManager content;

        public Dictionary<string, Level> CachedLevels { get; } = [];

        private static string CorrectName(string name)
        {
            string[] array = name.Split('/', '\\');
            return array[^1];
        }

        public LevelsCache()
        {
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
                        ContreJourDXConfig.AspectRatio.LevelsFolder,
                        name
                    ])
                ]), "xml");
                Stream stream = Mokus2DGame.FileLoader.OpenFile(path);
                level = LevelXmlReader.Read(stream);
                CachedLevels[name] = level;
            }
            return level;
        }
    }
}
