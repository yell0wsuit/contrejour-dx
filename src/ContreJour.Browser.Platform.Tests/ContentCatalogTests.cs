using System.IO;
using System.Text;

using Xunit;

namespace ContreJour.Browser.Platform.Tests
{
    public class ContentCatalogTests
    {
        [Fact]
        public void ReadsTheFourGroups()
        {
            byte[] json = Encoding.UTF8.GetBytes(/*lang=json,strict*/ """
                {"fonts":["f.ttf"],"images":["a.webp","b.webp"],"songs":["m.ogg"],"sounds":[]}
                """);

            ContentCatalog catalog = ContentCatalog.Read(json);

            Assert.Equal(["a.webp", "b.webp"], catalog.Images);
            Assert.Equal(["f.ttf"], catalog.Fonts);
            Assert.Empty(catalog.Sounds);
            Assert.Equal(["m.ogg"], catalog.Songs);
        }

        [Fact]
        public void AMissingGroupIsEmpty()
        {
            ContentCatalog catalog = ContentCatalog.Read(/*lang=json,strict*/ """{"images":["a.webp"]}"""u8);

            Assert.Empty(catalog.Songs);
        }

        [Fact]
        public void MalformedJsonIsRefused()
        {
            _ = Assert.Throws<InvalidDataException>(() => ContentCatalog.Read(/*lang=json,strict*/ "{\"images\":[1]}"u8));
        }
    }
}
