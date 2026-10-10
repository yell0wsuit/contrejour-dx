using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Xml.Linq;

using ContreJourDX.Gameplay;

namespace ContreJourDX.Content
{
    // Reads a level file. Every element names the Flash type it was exported from in __type__, and the five
    // types the levels use are built directly here. The serializer this replaces looked types up by name and
    // set members by reflection, which a trimmed build cannot do; keys are still read exactly as it read them.
    public static class LevelXmlReader
    {
        private const string TypeAttribute = "__type__";

        private const string IndexPrefix = "__";

        private static readonly Dictionary<string, string> KeyAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Level"] = typeof(Level).AssemblyQualifiedName,
            ["NSMutableArray"] = typeof(List<object>).FullName,
            ["NSMutableDictionary"] = typeof(Hashtable).FullName,
            ["FlashPoint"] = typeof(Vector2).AssemblyQualifiedName,
            ["x"] = "X",
            ["y"] = "Y",
            ["width"] = "Width",
            ["height"] = "Height",
            ["frames"] = "Frames",
            ["tileData"] = "TileData",
            ["anchor"] = "Anchor",
            ["useSheet"] = "UseSheet",
        };

        public static Level Read(Stream stream)
        {
            using StreamReader reader = new(stream);
            object root = Read(XDocument.Load(reader).Root);
            return root is null or Level
                ? (Level)root
                : throw new InvalidDataException($"A level file holds a {root.GetType().Name}, not a Level.");
        }

        public static object Read(XElement element)
        {
            string tag = element.Attribute(TypeAttribute)?.Value
                ?? throw new InvalidDataException($"<{element.Name}> has no {TypeAttribute}.");
            return tag == "null"
                ? null
                : IsTag(tag, "NSMutableDictionary")
                ? ReadDictionary(element)
                : IsTag(tag, "NSMutableArray")
                ? ReadList(element)
                : IsTag(tag, "FlashPoint")
                ? ReadPoint(element)
                : IsTag(tag, "Level")
                ? ReadLevel(element)
                : throw new InvalidDataException($"<{element.Name}> has the unknown type '{tag}'.");
        }

        private static Hashtable ReadDictionary(XElement element)
        {
            Hashtable dictionary = [];
            foreach ((string key, object value) in Members(element))
            {
                dictionary[key] = value;
            }
            return dictionary;
        }

        private static List<object> ReadList(XElement element)
        {
            List<object> list = [];
            foreach ((string key, object value) in Members(element))
            {
                int index = Convert.ToInt32(key, CultureInfo.InvariantCulture);
                while (list.Count < index + 1)
                {
                    list.Add(null);
                }
                list[index] = value;
            }
            return list;
        }

        private static Vector2 ReadPoint(XElement element)
        {
            Vector2 point = Vector2.Zero;
            foreach ((string key, object value) in Members(element))
            {
                float coordinate = value is string text
                    ? Convert.ToSingle(text, CultureInfo.InvariantCulture.NumberFormat)
                    : throw new InvalidDataException($"<{element.Name}> has a {key} that is not a number.");
                switch (key)
                {
                    case "X":
                        point.X = coordinate;
                        break;
                    case "Y":
                        point.Y = coordinate;
                        break;
                    default:
                        throw new InvalidDataException($"A FlashPoint has no member '{key}'.");
                }
            }
            return point;
        }

        private static Level ReadLevel(XElement element)
        {
            List<object> items = null;
            Hashtable properties = null;
            foreach ((string key, object value) in Members(element))
            {
                switch (key)
                {
                    case "items" when value is null or List<object>:
                        items = (List<object>)value;
                        break;
                    case "levelProperties" when value is null or Hashtable:
                        properties = (Hashtable)value;
                        break;
                    default:
                        throw new InvalidDataException($"A Level has no member '{key}' holding a {value?.GetType().Name ?? "null"}.");
                }
            }
            return new Level(items, properties);
        }

        private static IEnumerable<(string Key, object Value)> Members(XElement element)
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                if (attribute.Name != TypeAttribute)
                {
                    yield return (Key(attribute.Name.ToString()), attribute.Value);
                }
            }
            foreach (XNode node in element.Nodes())
            {
                XElement child = node as XElement
                    ?? throw new InvalidDataException($"<{element.Name}> holds a {node.NodeType} node.");
                yield return (Key(child.Name.ToString()), Read(child));
            }
        }

        private static string Key(string name)
        {
            return name.StartsWith(IndexPrefix, StringComparison.Ordinal)
                ? name[IndexPrefix.Length..]
                : KeyAliases.GetValueOrDefault(name, name);
        }

        private static bool IsTag(string tag, string name)
        {
            return string.Equals(tag, name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
