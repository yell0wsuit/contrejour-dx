using System.Collections.Generic;

namespace Default.Namespace;

public class Level
{
    // Set by the level XML serializer, which assigns fields by these names.
#pragma warning disable CS0649
    private readonly List<object> items;

    private readonly Hashtable levelProperties;
#pragma warning restore CS0649

    public List<object> Items => items;

    public Hashtable LevelProperties => levelProperties;
}
