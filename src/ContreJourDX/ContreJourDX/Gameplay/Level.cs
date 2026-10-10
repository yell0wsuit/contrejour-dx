using System.Collections.Generic;

namespace ContreJourDX.Gameplay
{
    public class Level(List<object> items, Hashtable levelProperties)
    {
        public List<object> Items { get; } = items;

        public Hashtable LevelProperties { get; } = levelProperties;
    }
}
