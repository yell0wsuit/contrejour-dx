using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ContreJour.Browser.Platform
{
    // content/assets.json (tools/webcontent/catalog.py): what the host preloads, by kind.
    public sealed record ContentCatalog(string[] Images, string[] Fonts, string[] Sounds, string[] Songs)
    {
        public static ContentCatalog Read(ReadOnlySpan<byte> json)
        {
            Dictionary<string, string[]> groups = [];
            try
            {
                Utf8JsonReader reader = new(json);
                Expect(reader.Read() && reader.TokenType == JsonTokenType.StartObject);
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    string name = reader.GetString();
                    Expect(reader.Read() && reader.TokenType == JsonTokenType.StartArray);
                    List<string> paths = [];
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        Expect(reader.TokenType == JsonTokenType.String);
                        paths.Add(reader.GetString());
                    }
                    groups[name] = [.. paths];
                }
            }
            catch (JsonException failure)
            {
                throw new InvalidDataException("assets.json is not valid JSON.", failure);
            }
            return new ContentCatalog(Group(groups, "images"), Group(groups, "fonts"), Group(groups, "sounds"), Group(groups, "songs"));
        }

        private static string[] Group(Dictionary<string, string[]> groups, string name)
        {
            return groups.GetValueOrDefault(name, []);
        }

        private static void Expect(bool condition)
        {
            if (!condition)
            {
                throw new InvalidDataException("assets.json must map group names to arrays of paths.");
            }
        }
    }
}
