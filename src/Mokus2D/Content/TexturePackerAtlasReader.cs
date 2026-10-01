using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text.Json;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Data;

namespace Mokus2D.Content
{
    // Reads a TexturePacker JSON atlas (the JSON Array or JSON Hash preset) into sprite and movie clip data.
    // A frame named "Name" is a sprite; frames named "Name/0000", "Name/0001"... are movie clip Name, played
    // in number order. Trimmed frames are supported; rotated ones are not, because quads are axis-aligned.
    public static class TexturePackerAtlasReader
    {
        private const string PngExtension = ".png";

        private static readonly Vector2 DefaultPivot = new(0.5f, 0.5f);

        public static List<TextureNodeData> Read(
            Stream json,
            string folder,
            float scaleFactor,
            string graphicsRoot,
            Func<string, ITexture> loadTexture,
            IReadOnlyDictionary<string, Dictionary<string, string>> configs,
            string fileName)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;
            string textureName = Path.Combine(graphicsRoot, folder + "/" + Path.GetFileNameWithoutExtension(ReadImage(root, fileName)));
            // The file suffix gives the art's scale; meta.scale is TexturePacker's packing scale on top of it,
            // so a "_x2" sheet packed at 0.78125 holds art at 1.5625 times the 1x size.
            scaleFactor /= ReadMetaScale(root, fileName);

            List<string> order = [];
            Dictionary<string, Frame> sprites = [];
            Dictionary<string, List<Frame>> clips = [];
            foreach ((string key, JsonElement element) in EnumerateFrames(root, fileName))
            {
                string name = key.EndsWith(PngExtension, StringComparison.OrdinalIgnoreCase) ? key[..^PngExtension.Length] : key;
                Frame frame = ReadFrame(element, name, fileName);
                int slash = name.IndexOf('/');
                if (slash < 0)
                {
                    if (sprites.ContainsKey(name) || clips.ContainsKey(name))
                    {
                        throw Error(fileName, key, sprites.ContainsKey(name) ? "appears more than once" : "is both a sprite and a movie clip");
                    }
                    sprites[name] = frame;
                    order.Add(name);
                    continue;
                }
                string clipName = name[..slash];
                if (clipName.Length == 0 || !int.TryParse(name.AsSpan(slash + 1), NumberStyles.None, CultureInfo.InvariantCulture, out int number))
                {
                    throw Error(fileName, key, "is not Name or Name/NNNN");
                }
                if (sprites.ContainsKey(clipName))
                {
                    throw Error(fileName, key, "is both a sprite and a movie clip");
                }
                if (!clips.TryGetValue(clipName, out List<Frame> frames))
                {
                    clips[clipName] = frames = [];
                    order.Add(clipName);
                }
                if (frames.Exists(f => f.Number == number))
                {
                    throw Error(fileName, key, "appears more than once");
                }
                frames.Add(frame with { Number = number });
            }

            List<TextureNodeData> result = new(order.Count);
            if (order.Count == 0)
            {
                return result;
            }
            ITexture texture = loadTexture(textureName);
            foreach (string name in order)
            {
                TextureNodeData data = sprites.TryGetValue(name, out Frame sprite)
                    ? CreateSprite(folder + "/" + name, sprite)
                    : CreateClip(folder + "/" + name, clips[name], fileName);
                data.ScaleFactor = scaleFactor;
                data.TextureName = textureName;
                data.Texture = texture;
                // A private copy per entry, as the XML loader built a fresh dictionary for every entry it read.
                data.Config = configs.TryGetValue(name, out Dictionary<string, string> config) ? new Dictionary<string, string>(config) : null;
                result.Add(data);
            }
            return result;
        }

        private static SpriteData CreateSprite(string id, Frame frame)
        {
            Rectangle rect = frame.Rect;
            // Untrimmed (every shipped sprite): the pivot is the anchor, bit for bit, with no arithmetic.
            Vector2 anchor = frame.OffsetX == 0 && frame.OffsetY == 0 && frame.SourceWidth == rect.Width && frame.SourceHeight == rect.Height
                ? frame.Pivot
                : new Vector2(
                    ((frame.Pivot.X * frame.SourceWidth) - frame.OffsetX) / rect.Width,
                    ((frame.Pivot.Y * frame.SourceHeight) - frame.OffsetY) / rect.Height);
            return new SpriteData(id) { Frame = new FrameData { Rect = rect, Anchor = anchor } };
        }

        private static MovieClipData CreateClip(string id, List<Frame> frames, string fileName)
        {
            frames.Sort((a, b) => a.Number.CompareTo(b.Number));
            Frame first = frames[0];
            MovieClipData data = new(id)
            {
                Anchor = first.Pivot,
                Size = new Vector2(first.SourceWidth, first.SourceHeight),
            };
            foreach (Frame frame in frames)
            {
                if (frame.Pivot != first.Pivot)
                {
                    throw Error(fileName, frame.Key, "has a different pivot from the clip's other frames");
                }
                if (frame.SourceWidth != first.SourceWidth || frame.SourceHeight != first.SourceHeight)
                {
                    throw Error(fileName, frame.Key, "has a different sourceSize from the clip's other frames");
                }
                // The engine anchors a clip frame by its offset inside the clip bounds, as a fraction of the
                // clip size. Negating the int first keeps a zero offset at +0f, as the old XML "0,0" parsed.
                data.Frames.Add(new FrameData
                {
                    Rect = frame.Rect,
                    Anchor = frame.Rect == Rectangle.Empty
                        ? Vector2.Zero
                        : new Vector2((float)-frame.OffsetX / frame.SourceWidth, (float)-frame.OffsetY / frame.SourceHeight),
                });
            }
            return data;
        }

        private static IEnumerable<(string Key, JsonElement Element)> EnumerateFrames(JsonElement root, string fileName)
        {
            if (!root.TryGetProperty("frames", out JsonElement frames))
            {
                throw new InvalidDataException($"{fileName}: the atlas has no frames block.");
            }
            if (frames.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in frames.EnumerateObject())
                {
                    yield return (property.Name, property.Value);
                }
                yield break;
            }
            if (frames.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException($"{fileName}: frames is neither an array nor an object.");
            }
            foreach (JsonElement element in frames.EnumerateArray())
            {
                JsonElement name = element.TryGetProperty("filename", out JsonElement candidate) && candidate.ValueKind == JsonValueKind.String
                    ? candidate
                    : throw new InvalidDataException($"{fileName}: a frame has no filename.");
                yield return (name.GetString(), element);
            }
        }

        private static string ReadImage(JsonElement root, string fileName)
        {
            return root.TryGetProperty("meta", out JsonElement meta)
                && meta.TryGetProperty("image", out JsonElement image)
                && image.ValueKind == JsonValueKind.String
                ? image.GetString()
                : throw new InvalidDataException($"{fileName}: the atlas has no meta.image.");
        }

        private static float ReadMetaScale(JsonElement root, string fileName)
        {
            if (!root.TryGetProperty("meta", out JsonElement meta) || !meta.TryGetProperty("scale", out JsonElement scale))
            {
                return 1f;
            }
            // TexturePacker writes the scale as a string; a plain number is accepted too.
            float value = float.NaN;
            _ = scale.ValueKind switch
            {
                JsonValueKind.String => float.TryParse(scale.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value),
                JsonValueKind.Number => scale.TryGetSingle(out value),
                _ => false,
            };
            return float.IsFinite(value) && value > 0f
                ? value
                : throw new InvalidDataException($"{fileName}: meta.scale must be a positive number.");
        }

        private static Frame ReadFrame(JsonElement element, string key, string fileName)
        {
            if (element.TryGetProperty("rotated", out JsonElement rotated) && rotated.ValueKind == JsonValueKind.True)
            {
                throw Error(fileName, key, "is rotated; turn rotation off in TexturePacker");
            }
            if (!element.TryGetProperty("frame", out JsonElement rectElement) || rectElement.ValueKind != JsonValueKind.Object)
            {
                throw Error(fileName, key, "has no frame rectangle");
            }
            Rectangle rect = new(ReadInt(rectElement, "x", key, fileName), ReadInt(rectElement, "y", key, fileName), ReadInt(rectElement, "w", key, fileName), ReadInt(rectElement, "h", key, fileName));

            int offsetX = 0;
            int offsetY = 0;
            if (element.TryGetProperty("spriteSourceSize", out JsonElement offset))
            {
                offsetX = ReadInt(offset, "x", key, fileName);
                offsetY = ReadInt(offset, "y", key, fileName);
            }
            int sourceWidth = rect.Width;
            int sourceHeight = rect.Height;
            if (element.TryGetProperty("sourceSize", out JsonElement source))
            {
                sourceWidth = ReadInt(source, "w", key, fileName);
                sourceHeight = ReadInt(source, "h", key, fileName);
            }
            // The digits parse straight to float, exactly as Convert.ToSingle did for the XML.
            Vector2 pivot = element.TryGetProperty("pivot", out JsonElement pivotElement)
                ? new Vector2(ReadFloat(pivotElement, "x", key, fileName), ReadFloat(pivotElement, "y", key, fileName))
                : DefaultPivot;
            return new Frame(key, 0, rect, offsetX, offsetY, sourceWidth, sourceHeight, pivot);
        }

        private static int ReadInt(JsonElement parent, string property, string key, string fileName)
        {
            return parent.ValueKind == JsonValueKind.Object && parent.TryGetProperty(property, out JsonElement value) && value.TryGetInt32(out int result)
                ? result
                : throw Error(fileName, key, $"has a missing or malformed {property}");
        }

        private static float ReadFloat(JsonElement parent, string property, string key, string fileName)
        {
            return parent.ValueKind == JsonValueKind.Object && parent.TryGetProperty(property, out JsonElement value) && value.TryGetSingle(out float result)
                ? result
                : throw Error(fileName, key, $"has a missing or malformed pivot {property}");
        }

        private static InvalidDataException Error(string fileName, string key, string problem)
        {
            return new InvalidDataException($"{fileName}: frame \"{key}\" {problem}.");
        }

        private readonly record struct Frame(string Key, int Number, Rectangle Rect, int OffsetX, int OffsetY, int SourceWidth, int SourceHeight, Vector2 Pivot);
    }
}
