using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ContreJour.Saving
{
    /// <summary>
    /// One save file: a flat key/value dictionary persisted as JSON, with a dirty mark so a save only
    /// rewrites files that changed.
    /// </summary>
    /// <param name="fileName">File name in the save directory.</param>
    internal sealed class PreferenceFile(string fileName)
    {
        private readonly Dictionary<string, object> data = [];

        /// <summary>Gets the file name in the save directory.</summary>
        public string FileName { get; } = fileName;

        /// <summary>Gets or sets a value indicating whether the data changed since it was last written.</summary>
        public bool Dirty { get; set; }

        /// <summary>Gets an integer value, or <paramref name="defaultValue"/> when it is missing or not a number.</summary>
        /// <param name="key">Preference key.</param>
        /// <param name="defaultValue">Value returned when the key is absent.</param>
        public int GetInt(string key, int defaultValue = 0)
        {
            return data.TryGetValue(key, out object value)
                ? value switch
                {
                    int intVal => intVal,
                    long longVal => (int)longVal,
                    _ => defaultValue
                }
                : defaultValue;
        }

        /// <summary>Sets an integer value.</summary>
        /// <param name="key">Preference key.</param>
        /// <param name="value">Value to store.</param>
        public void SetInt(string key, int value)
        {
            Set(key, value);
        }

        /// <summary>Gets a boolean value, or <paramref name="defaultValue"/> when it is missing.</summary>
        /// <param name="key">Preference key.</param>
        /// <param name="defaultValue">Value returned when the key is absent.</param>
        public bool GetBool(string key, bool defaultValue = false)
        {
            return data.TryGetValue(key, out object value) && value is bool boolVal ? boolVal : defaultValue;
        }

        /// <summary>Sets a boolean value.</summary>
        /// <param name="key">Preference key.</param>
        /// <param name="value">Value to store.</param>
        public void SetBool(string key, bool value)
        {
            Set(key, value);
        }

        /// <summary>Returns whether <paramref name="key"/> is present.</summary>
        /// <param name="key">Preference key.</param>
        public bool Contains(string key)
        {
            return data.ContainsKey(key);
        }

        /// <summary>Removes <paramref name="key"/>.</summary>
        /// <param name="key">Preference key.</param>
        public void Remove(string key)
        {
            // Taking a key away is as much a change as setting one; without the mark the removed
            // key would survive on disk.
            Dirty |= data.Remove(key);
        }

        private void Set(string key, object value)
        {
            // Marked on the mutation rather than on the save request: whatever requests the next
            // save has to write it.
            data[key] = value;
            Dirty = true;
        }

        /// <summary>Serializes the data as indented JSON, keys in ordinal order (AOT-safe).</summary>
        public string ToJson()
        {
            using MemoryStream stream = new();
            using (Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = true }))
            {
                writer.WriteStartObject();
                foreach (KeyValuePair<string, object> kvp in data.OrderBy(kvp => kvp.Key, StringComparer.Ordinal))
                {
                    writer.WritePropertyName(kvp.Key);
                    switch (kvp.Value)
                    {
                        case int intVal:
                            writer.WriteNumberValue(intVal);
                            break;
                        case long longVal:
                            writer.WriteNumberValue(longVal);
                            break;
                        case bool boolVal:
                            writer.WriteBooleanValue(boolVal);
                            break;
                        case string strVal:
                            writer.WriteStringValue(strVal);
                            break;
                        default:
                            writer.WriteNullValue();
                            break;
                    }
                }
                writer.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>Replaces the data with the contents of a JSON object (AOT-safe). Leaves it clean.</summary>
        /// <param name="json">JSON document to read.</param>
        public void LoadJson(string json)
        {
            data.Clear();
            Dirty = false;
            using JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            foreach (JsonProperty prop in doc.RootElement.EnumerateObject())
            {
                if (TryReadJsonValue(prop.Value, out object parsedValue))
                {
                    data[prop.Name] = parsedValue;
                }
            }
        }

        /// <summary>Clears the data, leaving it clean.</summary>
        public void Clear()
        {
            data.Clear();
            Dirty = false;
        }

        private static bool TryReadJsonValue(JsonElement element, out object parsedValue)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int intVal))
                    {
                        parsedValue = intVal;
                        return true;
                    }
                    if (element.TryGetInt64(out long longVal))
                    {
                        parsedValue = longVal;
                        return true;
                    }
                    break;
                case JsonValueKind.String:
                    parsedValue = element.GetString() ?? "";
                    return true;
                case JsonValueKind.True:
                    parsedValue = true;
                    return true;
                case JsonValueKind.False:
                    parsedValue = false;
                    return true;
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                case JsonValueKind.Null:
                default:
                    break;
            }

            parsedValue = null;
            return false;
        }
    }
}
