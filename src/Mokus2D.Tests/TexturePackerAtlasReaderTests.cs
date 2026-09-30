using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

using Mokus2D.Content;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Data;

using Xunit;

#pragma warning disable JSON002 // Tests intentionally compose incomplete JSON fragments.

namespace Mokus2D.Tests
{
    public class TexturePackerAtlasReaderTests
    {
        private static readonly Dictionary<string, Dictionary<string, string>> NoConfigs = [];

        private static List<TextureNodeData> Read(string json, IReadOnlyDictionary<string, Dictionary<string, string>> configs = null, List<string> loaded = null)
        {
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(json));
            return TexturePackerAtlasReader.Read(stream, "common", 0.5f, "Graphics", name =>
            {
                loaded?.Add(name);
                return new StubTexture();
            }, configs ?? NoConfigs, "common/common_x2.json");
        }

        private static string Atlas(string frames)
        {
            return "{\"frames\": " + frames + ", \"meta\": {\"image\": \"common_x2.png\", \"scale\": \"1\"}}";
        }

        private static string Frame(string name, int x, int y, int w, int h, string extra = "")
        {
            return $"{{\"filename\": \"{name}\", \"frame\": {{\"x\": {x}, \"y\": {y}, \"w\": {w}, \"h\": {h}}}{extra}}}";
        }

        private static void AssertBits(float expected, float actual)
        {
            Assert.Equal(BitConverter.SingleToInt32Bits(expected), BitConverter.SingleToInt32Bits(actual));
        }

        [Fact]
        public void UntrimmedSpriteKeepsThePivotBits()
        {
            string pivot = ", \"sourceSize\": {\"w\": 122, \"h\": 263}, \"spriteSourceSize\": {\"x\": 0, \"y\": 0, \"w\": 122, \"h\": 263}, \"pivot\": {\"x\": 0.4918032786885246, \"y\": 0.41064638783269963}";
            SpriteData sprite = Assert.IsType<SpriteData>(Assert.Single(Read(Atlas("[" + Frame("McArrowView", 1844, 1, 122, 263, pivot) + "]"))));

            Assert.Equal("common/McArrowView", sprite.Id);
            Assert.Equal(new Rectangle(1844, 1, 122, 263), sprite.Frame.Rect);
            AssertBits(float.Parse("0.4918032786885246", CultureInfo.InvariantCulture), sprite.Frame.Anchor.X);
            AssertBits(float.Parse("0.41064638783269963", CultureInfo.InvariantCulture), sprite.Frame.Anchor.Y);
            Assert.Equal(0.5f, sprite.ScaleFactor);
            Assert.Equal(Path.Combine("Graphics", "common/common_x2"), sprite.TextureName);
            Assert.NotNull(sprite.Texture);
            Assert.Null(sprite.Config);
        }

        [Fact]
        public void TrimmedSpriteMovesItsAnchorOntoTheTrimmedRect()
        {
            string trim = ", \"trimmed\": true, \"sourceSize\": {\"w\": 30, \"h\": 40}, \"spriteSourceSize\": {\"x\": 5, \"y\": 10, \"w\": 10, \"h\": 20}, \"pivot\": {\"x\": 0.5, \"y\": 0.5}";
            SpriteData sprite = Assert.IsType<SpriteData>(Assert.Single(Read(Atlas("[" + Frame("McFoo", 0, 0, 10, 20, trim) + "]"))));

            // Pivot (15, 20) in the 30x40 source, minus the (5, 10) trim offset, over the 10x20 rect.
            Assert.Equal(new Vector2(1f, 0.5f), sprite.Frame.Anchor);
        }

        [Fact]
        public void MissingPivotDefaultsToTheCenter()
        {
            SpriteData sprite = Assert.IsType<SpriteData>(Assert.Single(Read(Atlas("[" + Frame("McFoo", 0, 0, 10, 20) + "]"))));

            Assert.Equal(new Vector2(0.5f, 0.5f), sprite.Frame.Anchor);
        }

        [Fact]
        public void PngExtensionIsDroppedFromNames()
        {
            List<TextureNodeData> result = Read(Atlas("[" + Frame("McFoo.png", 0, 0, 1, 1) + ", " + Frame("McClip/0000.png", 1, 0, 1, 1) + "]"));

            Assert.Equal(["common/McFoo", "common/McClip"], result.ConvertAll(d => d.Id));
        }

        [Fact]
        public void ClipFramesAreOrderedByNumberNotByName()
        {
            string clip = ", \"sourceSize\": {\"w\": 112, \"h\": 106}, \"spriteSourceSize\": {\"x\": 9, \"y\": 6, \"w\": 1, \"h\": 1}, \"pivot\": {\"x\": 0.5, \"y\": 0.49056603773584906}";
            string frames = "[" + Frame("Clip/10", 10, 0, 1, 1, clip) + ", " + Frame("Clip/9", 9, 0, 1, 1, clip) + ", " + Frame("Clip/0", 0, 0, 1, 1, clip) + "]";
            MovieClipData data = Assert.IsType<MovieClipData>(Assert.Single(Read(Atlas(frames))));

            Assert.Equal("common/Clip", data.Id);
            Assert.Equal([0, 9, 10], data.Frames.ConvertAll(f => f.Rect.X));
            Assert.Equal(new Vector2(112f, 106f), data.Size);
            AssertBits(float.Parse("0.49056603773584906", CultureInfo.InvariantCulture), data.Anchor.Y);
            AssertBits(-9f / 112f, data.Frames[0].Anchor.X);
            AssertBits(-6f / 106f, data.Frames[0].Anchor.Y);
        }

        [Fact]
        public void UntrimmedAndEmptyClipFramesHavePositiveZeroAnchors()
        {
            string full = ", \"sourceSize\": {\"w\": 8, \"h\": 8}, \"spriteSourceSize\": {\"x\": 0, \"y\": 0, \"w\": 8, \"h\": 8}";
            string empty = ", \"sourceSize\": {\"w\": 8, \"h\": 8}, \"spriteSourceSize\": {\"x\": 0, \"y\": 0, \"w\": 0, \"h\": 0}";
            MovieClipData data = Assert.IsType<MovieClipData>(Assert.Single(Read(Atlas("[" + Frame("Clip/0000", 4, 4, 8, 8, full) + ", " + Frame("Clip/0001", 0, 0, 0, 0, empty) + "]"))));

            AssertBits(0f, data.Frames[0].Anchor.X);
            AssertBits(0f, data.Frames[0].Anchor.Y);
            Assert.Equal(Rectangle.Empty, data.Frames[1].Rect);
            AssertBits(0f, data.Frames[1].Anchor.X);
            AssertBits(0f, data.Frames[1].Anchor.Y);
        }

        [Fact]
        public void HashAndArrayGiveTheSameResult()
        {
            string pivot = ", \"pivot\": {\"x\": 0.25, \"y\": 0.75}";
            List<TextureNodeData> array = Read(Atlas("[" + Frame("A", 1, 2, 3, 4, pivot) + ", " + Frame("B", 5, 6, 7, 8) + "]"));
            List<TextureNodeData> hash = Read(Atlas("{\"A\": {\"frame\": {\"x\": 1, \"y\": 2, \"w\": 3, \"h\": 4}" + pivot + "}, \"B\": {\"frame\": {\"x\": 5, \"y\": 6, \"w\": 7, \"h\": 8}}}"));

            Assert.Equal(array.ConvertAll(d => d.Id), hash.ConvertAll(d => d.Id));
            for (int i = 0; i < array.Count; i++)
            {
                Assert.Equal(((SpriteData)array[i]).Frame, ((SpriteData)hash[i]).Frame);
            }
        }

        [Fact]
        public void EntriesKeepFirstAppearanceOrderAndShareOneTextureLoad()
        {
            List<string> loaded = [];
            List<TextureNodeData> result = Read(Atlas("[" + Frame("B", 0, 0, 1, 1) + ", " + Frame("Clip/0000", 0, 0, 1, 1) + ", " + Frame("A", 0, 0, 1, 1) + ", " + Frame("Clip/0001", 0, 0, 1, 1) + "]"), loaded: loaded);

            Assert.Equal(["common/B", "common/Clip", "common/A"], result.ConvertAll(d => d.Id));
            Assert.Equal([Path.Combine("Graphics", "common/common_x2")], loaded);
            Assert.Same(result[0].Texture, result[2].Texture);
        }

        [Fact]
        public void EmptyAtlasLoadsNoTexture()
        {
            List<string> loaded = [];

            Assert.Empty(Read(Atlas("[]"), loaded: loaded));
            Assert.Empty(loaded);
        }

        [Fact]
        public void ConfigIsAttachedByNameAsAPrivateCopy()
        {
            Dictionary<string, Dictionary<string, string>> configs = new() { ["A"] = new() { ["z"] = "-8", ["type"] = "null" } };
            List<TextureNodeData> first = Read(Atlas("[" + Frame("A", 0, 0, 1, 1) + ", " + Frame("B", 0, 0, 1, 1) + "]"), configs);
            List<TextureNodeData> second = Read(Atlas("[" + Frame("A", 0, 0, 1, 1) + "]"), configs);

            Assert.Equal(["z", "type"], first[0].Config.Keys);
            Assert.Equal("-8", first[0].Config["z"]);
            Assert.Null(first[1].Config);
            Assert.NotSame(configs["A"], first[0].Config);
            Assert.NotSame(first[0].Config, second[0].Config);
        }

        [Theory]
        [InlineData("[{\"filename\": \"A\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}, \"rotated\": true}]", "rotated")]
        [InlineData("[{\"filename\": \"A\"}]", "frame")]
        [InlineData("[{\"filename\": \"A/B/0000\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}]", "Name/NNNN")]
        [InlineData("[{\"filename\": \"A/x\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}]", "Name/NNNN")]
        [InlineData("[{\"filename\": \"A\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}, {\"filename\": \"A\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}]", "more than once")]
        [InlineData("[{\"filename\": \"A/0\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}, {\"filename\": \"A/00\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}]", "more than once")]
        [InlineData("[{\"filename\": \"A\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}, {\"filename\": \"A/0\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}]", "both a sprite and a movie clip")]
        [InlineData("[{\"filename\": \"A/0\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}, \"pivot\": {\"x\": 0, \"y\": 0}}, {\"filename\": \"A/1\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}]", "pivot")]
        [InlineData("[{\"filename\": \"A/0\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 1, \"h\": 1}}, {\"filename\": \"A/1\", \"frame\": {\"x\": 0, \"y\": 0, \"w\": 2, \"h\": 1}}]", "sourceSize")]
        public void BadFramesAreRejectedWithTheFileAndEntry(string frames, string reason)
        {
            InvalidDataException error = Assert.Throws<InvalidDataException>(() => Read(Atlas(frames)));

            Assert.Contains("common/common_x2.json", error.Message, StringComparison.Ordinal);
            Assert.Contains(reason, error.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void MissingFramesOrImageIsRejected()
        {
            _ = Assert.Throws<InvalidDataException>(() => Read("{\"meta\": {\"image\": \"common_x2.png\"}}"));
            _ = Assert.Throws<InvalidDataException>(() => Read("{\"frames\": [" + Frame("A", 0, 0, 1, 1) + "], \"meta\": {}}"));
        }
    }
}

#pragma warning restore JSON002
