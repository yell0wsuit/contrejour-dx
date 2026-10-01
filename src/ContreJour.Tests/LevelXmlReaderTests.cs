using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Xml.Linq;

using ContreJour.Content;
using ContreJour.Gameplay;

using Xunit;

namespace ContreJour.Tests
{
    public class LevelXmlReaderTests
    {
        private static object Read(string xml)
        {
            return LevelXmlReader.Read(XElement.Parse(xml));
        }

        private static Hashtable ReadDictionary(string attributesAndChildren)
        {
            return Assert.IsType<Hashtable>(Read($"<d __type__=\"NSMutableDictionary\" {attributesAndChildren}"));
        }

        [Fact]
        public void ReadsALevel()
        {
            const string xml = """
                <level __type__="Level">
                  <levelProperties __type__="NSMutableDictionary" width="1024" name="test" />
                  <items __type__="NSMutableArray">
                    <__0 __type__="FlashPoint" x="1.5" y="-2" />
                  </items>
                </level>
                """;
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(xml));

            Level level = LevelXmlReader.Read(stream);

            Assert.Equal(["Width", "name"], level.LevelProperties.Keys);
            Assert.Equal("1024", level.LevelProperties["Width"]);
            Assert.Equal(new Vector2(1.5f, -2f), Assert.Single(level.Items));
        }

        [Fact]
        public void AMissingLevelMemberStaysNull()
        {
            Level level = Assert.IsType<Level>(Read("<level __type__=\"Level\"><items __type__=\"NSMutableArray\" /></level>"));

            Assert.Empty(level.Items);
            Assert.Null(level.LevelProperties);
        }

        [Fact]
        public void DictionaryKeysAreRenamedIgnoringCase()
        {
            Hashtable d = ReadDictionary("x=\"1\" Y=\"2\" WIDTH=\"3\" height=\"4\" frames=\"5\" tiledata=\"6\" anchor=\"7\" usesheet=\"8\" other=\"9\" />");

            Assert.Equal(["X", "Y", "Width", "Height", "Frames", "TileData", "Anchor", "UseSheet", "other"], d.Keys);
        }

        [Fact]
        public void KeySpelledLikeATypeTagBecomesThatTypesName()
        {
            Hashtable d = ReadDictionary("level=\"1\" />");

            Assert.Equal([typeof(Level).AssemblyQualifiedName], d.Keys);
        }

        [Fact]
        public void PrefixedKeysAreNotRenamed()
        {
            Hashtable d = ReadDictionary("__x=\"1\" />");

            Assert.Equal(["x"], d.Keys);
        }

        [Fact]
        public void ChildrenFollowAttributesAndLaterKeysWin()
        {
            Hashtable d = ReadDictionary("x=\"attribute\" b=\"2\"><X __type__=\"null\" /><c __type__=\"NSMutableArray\" /></d>");

            Assert.Equal(["X", "b", "c"], d.Keys);
            Assert.Null(d["X"]);
            _ = Assert.IsType<List<object>>(d["c"]);
        }

        [Fact]
        public void ListsArePaddedWithNulls()
        {
            List<object> list = Assert.IsType<List<object>>(Read("<l __type__=\"NSMutableArray\"><__2 __type__=\"FlashPoint\" x=\"1\" /></l>"));

            Assert.Equal([null, null, new Vector2(1f, 0f)], list);
        }

        [Fact]
        public void TypeTagsIgnoreCase()
        {
            _ = Assert.IsType<List<object>>(Read("<l __type__=\"nsmutablearray\" />"));
            _ = Assert.IsType<Vector2>(Read("<p __type__=\"flashpoint\" />"));
        }

        [Fact]
        public void PointsParseInvariantNumbers()
        {
            Assert.Equal(new Vector2(1000.5f, 1000f), Read("<p __type__=\"FlashPoint\" x=\"1,000.5\" y=\"1e3\" />"));
        }

        [Fact]
        public void NullTagReadsAsNull()
        {
            Assert.Null(Read("<n __type__=\"null\" />"));
        }

        [Theory]
        [InlineData("<a __type__=\"Rectangle\" />")]
        [InlineData("<a />")]
        [InlineData("<l __type__=\"Level\" name=\"x\" />")]
        [InlineData("<p __type__=\"FlashPoint\" z=\"1\" />")]
        [InlineData("<d __type__=\"NSMutableDictionary\">text</d>")]
        public void MalformedElementsThrow(string xml)
        {
            _ = Assert.Throws<InvalidDataException>(() => Read(xml));
        }
    }
}
