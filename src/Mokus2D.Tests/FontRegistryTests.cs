using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

using Mokus2D.FileSystem;
using Mokus2D.Visual.Text;

using Xunit;

namespace Mokus2D.Tests
{
    public class FontRegistryTests
    {
        private const string Folder = "Assets/Content/fonts";

        private const string Config = /*lang=json,strict*/ """
            {
              "default": { "file": "Latin.ttf" },
              "ru": { "file": "Cyrillic.ttf", "scale": 1.5 }
            }
            """;

        private sealed class MemoryFiles(Dictionary<string, string> files) : IFileLoader
        {
            public Stream OpenFile(string path)
            {
                string key = path.Replace('\\', '/');
                return files.TryGetValue(key, out string text) ? new MemoryStream(Encoding.UTF8.GetBytes(text)) : throw new FileNotFoundException(key);
            }
        }

        // Each font file holds its own name, so the stub face records which file it was made from.
        private static MemoryFiles Files(string json, params string[] fontFiles)
        {
            Dictionary<string, string> files = [];
            if (json != null)
            {
                files[$"{Folder}/fonts.json"] = json;
            }
            foreach (string font in fontFiles)
            {
                files[$"{Folder}/{font}"] = font;
            }
            return new MemoryFiles(files);
        }

        private static FontRegistry Load(string json, string locale, params string[] fontFiles)
        {
            return FontRegistry.Load(Files(json, fontFiles), Folder, locale, stream => new StubFontFace(new StreamReader(stream).ReadToEnd()));
        }

        [Fact]
        public void PicksTheLocaleEntry()
        {
            using FontRegistry fonts = Load(Config, "ru", "Latin.ttf", "Cyrillic.ttf");

            Assert.Equal("Cyrillic.ttf", fonts.FontFile);
            Assert.Equal("Cyrillic.ttf", ((StubFontFace)fonts.Face).Source);
            Assert.Equal(1.5f, fonts.Scale);
        }

        [Theory]
        [InlineData("en")]
        [InlineData("xx")]
        [InlineData("")]
        [InlineData(null)]
        public void OtherLocalesUseTheDefault(string locale)
        {
            using FontRegistry fonts = Load(Config, locale, "Latin.ttf", "Cyrillic.ttf");

            Assert.Equal("Latin.ttf", ((StubFontFace)fonts.Face).Source);
            Assert.Equal(1f, fonts.Scale);
        }

        [Fact]
        public void LocaleKeysIgnoreCase()
        {
            using FontRegistry fonts = Load(Config, "RU", "Latin.ttf", "Cyrillic.ttf");

            Assert.Equal("Cyrillic.ttf", fonts.FontFile);
        }

        [Fact]
        public void MissingFilesThrowFileNotFound()
        {
            _ = Assert.Throws<FileNotFoundException>(() => Load(null, "en", "Latin.ttf"));
            _ = Assert.Throws<FileNotFoundException>(() => Load(Config, "en"));
        }

        [Fact]
        public void MalformedJsonThrowsJsonException()
        {
            _ = Assert.ThrowsAny<JsonException>(() => Load("{ \"default\": ", "en", "Latin.ttf"));
        }

        [Theory]
        [InlineData("[]")]
        [InlineData(/*lang=json,strict*/ """{ "ru": { "file": "Cyrillic.ttf" } }""")]
        [InlineData(/*lang=json,strict*/ """{ "default": "Latin.ttf" }""")]
        [InlineData(/*lang=json,strict*/ """{ "default": { "scale": 1 } }""")]
        [InlineData(/*lang=json,strict*/ """{ "default": { "file": " " } }""")]
        [InlineData(/*lang=json,strict*/ """{ "default": { "file": "Latin.ttf", "scale": 0 } }""")]
        [InlineData(/*lang=json,strict*/ """{ "default": { "file": "Latin.ttf", "scale": -1 } }""")]
        [InlineData(/*lang=json,strict*/ """{ "default": { "file": "Latin.ttf", "scale": "big" } }""")]
        public void InvalidConfigsThrowInvalidData(string json)
        {
            _ = Assert.Throws<InvalidDataException>(() => Load(json, "en", "Latin.ttf", "Cyrillic.ttf"));
        }

        [Fact]
        public void UnreadableFontsAreNotHidden()
        {
            _ = Assert.Throws<InvalidDataException>(() => FontRegistry.Load(Files(Config, "Latin.ttf"), Folder, "en", _ => throw new InvalidDataException()));
        }

        [Fact]
        public void LineHeightKeepsSegoePrintsRatioTimesScale()
        {
            using FontRegistry latin = Load(Config, "en", "Latin.ttf");
            using FontRegistry cyrillic = Load(Config, "ru", "Cyrillic.ttf");

            Assert.Equal(48.7f, latin.GetLineHeight(28f), 0.001f);
            Assert.Equal(48.7f * 1.5f, cyrillic.GetLineHeight(28f), 0.001f);
        }

        [Fact]
        public void EmSizeMakesTheFaceFillTheLineHeight()
        {
            using FontRegistry fonts = Load(Config, "en", "Latin.ttf");

            // The stub's line is exactly its size tall.
            Assert.Equal(48.7f, fonts.GetEmSize(48.7f), 0.001f);
        }

        [Fact]
        public void DisposingReleasesTheFace()
        {
            FontRegistry fonts = Load(Config, "en", "Latin.ttf");

            fonts.Dispose();

            Assert.True(((StubFontFace)fonts.Face).IsDisposed);
        }
    }
}
