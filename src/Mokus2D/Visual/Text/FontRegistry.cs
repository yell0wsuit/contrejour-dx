using System;
using System.IO;
using System.Text.Json;

using Mokus2D.FileSystem;
using Mokus2D.Graphics;

namespace Mokus2D.Visual.Text
{
    // The font face labels draw with, picked for the current locale from <folder>/fonts.json:
    //   { "default": { "file": "PatrickHand-Regular.ttf", "scale": 1.0 }, "ru": { "file": "Neucha.ttf" }, ... }
    // Keys are two-letter language codes, matched ignoring case; "default" covers every other locale.
    // "scale" is optional (1 when absent) and trims one face's size against the others.
    public sealed class FontRegistry : IDisposable
    {
        public const string ConfigFileName = "fonts.json";

        private const string DefaultKey = "default";

        // Segoe Print, the bitmap font this replaced, was 48.7 units tall at size 28. Keeping that line
        // height per unit of size keeps every label's vertical layout where it was.
        public const float LineHeightPerFontSize = 48.7f / 28f;

        // Metrics are read at a large size so the em size does not inherit their rounding.
        private const float MeasureSize = 100f;

        private FontRegistry(string fontFile, float scale, IFontFace face)
        {
            FontFile = fontFile;
            Scale = scale;
            Face = face;
        }

        public string FontFile { get; }

        public float Scale { get; }

        public IFontFace Face { get; }

        public static FontRegistry Load(IFileLoader files, string folder, string locale, Func<Stream, IFontFace> createFace)
        {
            ArgumentNullException.ThrowIfNull(files);
            ArgumentNullException.ThrowIfNull(createFace);
            string configPath = Path.Combine(folder, ConfigFileName);
            string fontFile;
            float scale;
            using (Stream json = files.OpenFile(configPath))
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    throw Error(configPath, "must be a JSON object");
                }
                if (!TryFindEntry(root, locale, out string key, out JsonElement entry)
                    && !TryFindEntry(root, DefaultKey, out key, out entry))
                {
                    throw Error(configPath, $"has no \"{DefaultKey}\" entry");
                }
                (fontFile, scale) = ReadEntry(entry, configPath, key);
            }
            using Stream font = files.OpenFile(Path.Combine(folder, fontFile));
            return new FontRegistry(fontFile, scale, createFace(font));
        }

        public float GetLineHeight(float fontSize)
        {
            return LineHeightPerFontSize * fontSize * Scale;
        }

        // The em size at which the face's ascent plus descent equals lineHeight (cuttherope-dx's rule).
        public float GetEmSize(float lineHeight)
        {
            FontMetrics metrics = Face.GetMetrics(MeasureSize);
            return lineHeight * MeasureSize / (metrics.Descent - metrics.Ascent);
        }

        public void Dispose()
        {
            Face.Dispose();
        }

        private static bool TryFindEntry(JsonElement root, string wanted, out string key, out JsonElement entry)
        {
            if (!string.IsNullOrEmpty(wanted))
            {
                foreach (JsonProperty property in root.EnumerateObject())
                {
                    if (string.Equals(property.Name, wanted, StringComparison.OrdinalIgnoreCase))
                    {
                        key = property.Name;
                        entry = property.Value;
                        return true;
                    }
                }
            }
            key = null;
            entry = default;
            return false;
        }

        private static (string FontFile, float Scale) ReadEntry(JsonElement entry, string configPath, string key)
        {
            if (entry.ValueKind != JsonValueKind.Object)
            {
                throw Error(configPath, $"entry \"{key}\" must be an object");
            }
            if (!entry.TryGetProperty("file", out JsonElement file) || file.ValueKind != JsonValueKind.String
                || string.IsNullOrWhiteSpace(file.GetString()))
            {
                throw Error(configPath, $"entry \"{key}\" needs a \"file\" name");
            }
            float scale = 1f;
            if (entry.TryGetProperty("scale", out JsonElement scaleElement))
            {
                if (scaleElement.ValueKind != JsonValueKind.Number || !scaleElement.TryGetSingle(out scale)
                    || !float.IsFinite(scale) || scale <= 0f)
                {
                    throw Error(configPath, $"entry \"{key}\" needs a positive \"scale\"");
                }
            }
            return (file.GetString(), scale);
        }

        private static InvalidDataException Error(string configPath, string problem)
        {
            return new InvalidDataException($"{configPath} {problem}.");
        }
    }
}
