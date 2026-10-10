using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ContreJourDX.Browser.Platform
{
    // Reads content/metadata.json (tools/webcontent/metadata.py): one JSON object mapping each XML or JSON
    // file's path to its text, verbatim. Each entry is handed out as the file's UTF-8 bytes.
    public static class MetadataBundle
    {
        public static void Read(ReadOnlySpan<byte> json, Action<string, byte[]> add)
        {
            ArgumentNullException.ThrowIfNull(add);
            try
            {
                Utf8JsonReader reader = new(json);
                Expect(reader.Read() && reader.TokenType == JsonTokenType.StartObject);
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    string path = reader.GetString();
                    Expect(reader.Read() && reader.TokenType == JsonTokenType.String);
                    add(path, Encoding.UTF8.GetBytes(reader.GetString()));
                }
            }
            catch (JsonException failure)
            {
                throw new InvalidDataException("metadata.json is not valid JSON.", failure);
            }
        }

        private static void Expect(bool condition)
        {
            if (!condition)
            {
                throw new InvalidDataException("metadata.json must be an object of strings.");
            }
        }
    }
}
