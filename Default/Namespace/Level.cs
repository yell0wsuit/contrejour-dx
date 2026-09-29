using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Default.Namespace;

public class Level
{
    // Set by the level XML serializer, which assigns fields by these names, so they can't become
    // auto-properties.
#pragma warning disable CS0649
    [SuppressMessage("Style", "IDE0032:Use auto property", Justification = "The level serializer sets this field by name.")]
    private readonly List<object> items;

    [SuppressMessage("Style", "IDE0032:Use auto property", Justification = "The level serializer sets this field by name.")]
    private readonly Hashtable levelProperties;
#pragma warning restore CS0649

    public List<object> Items => items;

    public Hashtable LevelProperties => levelProperties;
}
