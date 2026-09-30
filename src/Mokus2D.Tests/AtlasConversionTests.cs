using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

using Mokus2D.Content;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Data;

using Xunit;

namespace Mokus2D.Tests
{
    // Proves the one-off XML → JSON conversion lost nothing: the old sprites*.xml, parsed with the rules of
    // GraphicsDeserializerBase, must equal what the JSON reader returns. Deleted with the XML files.
    public class AtlasConversionTests
    {
        private const string GraphicsRoot = "Graphics";

        public static TheoryData<string, string> Atlases()
        {
            TheoryData<string, string> data = [];
            foreach (string folder in Directory.GetDirectories(GraphicsDirectory()).Select(Path.GetFileName).Order(StringComparer.Ordinal))
            {
                if (folder == "segoeFont" || !File.Exists(Path.Combine(GraphicsDirectory(), folder, "sprites.xml")))
                {
                    continue;
                }
                data.Add(folder, "");
                data.Add(folder, "_x2");
            }
            return data;
        }

        [Theory]
        [MemberData(nameof(Atlases))]
        public void JsonMatchesTheOldXml(string folder, string suffix)
        {
            string directory = Path.Combine(GraphicsDirectory(), folder);
            XElement[] expected = [.. XDocument.Load(Path.Combine(directory, "sprites" + suffix + ".xml")).Root.Elements()];
            string jsonPath = Path.Combine(directory, folder + suffix + ".json");
            if (expected.Length == 0)
            {
                Assert.False(File.Exists(jsonPath), $"{jsonPath} should not exist: the XML is empty");
                return;
            }

            string viewsPath = Path.Combine(directory, "views.xml");
            Dictionary<string, Dictionary<string, string>> configs = ViewConfigReader.Read(File.Exists(viewsPath) ? XDocument.Load(viewsPath) : null);
            List<TextureNodeData> actual;
            using (FileStream stream = File.OpenRead(jsonPath))
            {
                actual = TexturePackerAtlasReader.Read(stream, folder, suffix.Length == 0 ? 1f : 0.5f, GraphicsRoot, _ => new StubTexture(), configs, jsonPath);
            }

            Assert.Equal(expected.Select(e => folder + "/" + e.Name.LocalName), actual.Select(d => d.Id));
            for (int i = 0; i < expected.Length; i++)
            {
                AssertEntry(folder, suffix, expected[i], actual[i]);
            }
        }

        private static void AssertEntry(string folder, string suffix, XElement xml, TextureNodeData actual)
        {
            string id = actual.Id;
            Assert.Equal(float.Parse((string)xml.Attribute("scaleFactor") ?? "1", CultureInfo.InvariantCulture), actual.ScaleFactor);
            Assert.Equal(Path.Combine(GraphicsRoot, (string)xml.Attribute("texture")), actual.TextureName);

            Dictionary<string, string> expectedConfig = xml.Element("config")?.Attributes().ToDictionary(a => a.Name.ToString(), a => a.Value);
            if (folder == "chapter2" && suffix.Length == 0 && xml.Name.LocalName == "McSpringViewBlack")
            {
                // The one agreed difference: 1× takes the 2× value (no config).
                Assert.NotNull(expectedConfig);
                Assert.Null(actual.Config);
            }
            else if (expectedConfig == null)
            {
                Assert.Null(actual.Config);
            }
            else
            {
                Assert.Equal([.. expectedConfig], [.. actual.Config]);
            }

            switch ((string)xml.Attribute("type"))
            {
                case "sprite":
                    SpriteData sprite = Assert.IsType<SpriteData>(actual);
                    AssertFrame(id, xml.Element("frame"), sprite.Frame);
                    break;
                case "movieClip":
                    MovieClipData clip = Assert.IsType<MovieClipData>(actual);
                    Vector2 expectedAnchor = id == "menu2/McPlanetFur" ? Vector2.Zero : Vector((string)xml.Attribute("anchor"));
                    AssertBits(id, expectedAnchor, clip.Anchor);
                    AssertBits(id, Vector((string)xml.Attribute("size")), clip.Size);
                    XElement[] frames = [.. xml.Elements("frame")];
                    Assert.Equal(frames.Length, clip.Frames.Count);
                    for (int i = 0; i < frames.Length; i++)
                    {
                        AssertFrame($"{id}#{i}", frames[i], clip.Frames[i]);
                    }
                    break;
                default:
                    Assert.Fail($"{id}: unexpected type {(string)xml.Attribute("type")}");
                    break;
            }
        }

        private static void AssertFrame(string id, XElement xml, FrameData actual)
        {
            Assert.True(Rect((string)xml.Attribute("rect")) == actual.Rect, $"{id}: rect {actual.Rect.X},{actual.Rect.Y} {actual.Rect.Width}x{actual.Rect.Height}");
            AssertBits(id, Vector((string)xml.Attribute("anchor")), actual.Anchor);
        }

        private static void AssertBits(string id, Vector2 expected, Vector2 actual)
        {
            Assert.True(
                BitConverter.SingleToInt32Bits(expected.X) == BitConverter.SingleToInt32Bits(actual.X)
                && BitConverter.SingleToInt32Bits(expected.Y) == BitConverter.SingleToInt32Bits(actual.Y),
                $"{id}: expected {expected.X:R},{expected.Y:R} got {actual.X:R},{actual.Y:R}");
        }

        // GraphicsDeserializerBase.VectorFromString.
        private static Vector2 Vector(string value)
        {
            if (value == "null")
            {
                return Vector2.Zero;
            }
            string[] parts = value.Split(',');
            return new Vector2(Convert.ToSingle(parts[0], CultureInfo.InvariantCulture.NumberFormat), Convert.ToSingle(parts[1], CultureInfo.InvariantCulture.NumberFormat));
        }

        // GraphicsDeserializerBase.RectFromString.
        private static Rectangle Rect(string value)
        {
            if (value == "null")
            {
                return Rectangle.Empty;
            }
            string[] parts = value.Split(' ');
            Vector2 position = Vector(parts[0]);
            string[] size = parts[1].Split('x');
            return new Rectangle((int)position.X, (int)position.Y, Convert.ToInt32(size[0], CultureInfo.InvariantCulture), Convert.ToInt32(size[1], CultureInfo.InvariantCulture));
        }

        private static string GraphicsDirectory()
        {
            DirectoryInfo directory = new(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "ContreJour.slnx")))
            {
                directory = directory.Parent;
            }
            return directory == null
                ? throw new DirectoryNotFoundException("ContreJour.slnx not found above " + AppContext.BaseDirectory)
                : Path.Combine(directory.FullName, "GameContents", "Content", "Graphics");
        }
    }
}
