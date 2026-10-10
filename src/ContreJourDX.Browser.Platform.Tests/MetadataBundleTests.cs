using System.Collections.Generic;
using System.IO;
using System.Text;

using Xunit;

namespace ContreJourDX.Browser.Platform.Tests
{
    public class MetadataBundleTests
    {
        [Fact]
        public void EveryEntryArrivesAsUtf8BytesWithItsBom()
        {
            Dictionary<string, byte[]> added = [];
            byte[] json = Encoding.UTF8.GetBytes(/*lang=json,strict*/ "{\"Resources/values.ja.xml\":\"\\ufeff<r>日本語</r>\",\"a.json\":\"{}\"}");

            MetadataBundle.Read(json, (path, bytes) => added[path] = bytes);

            byte[] expected = [0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("<r>日本語</r>")];
            Assert.Equal(2, added.Count);
            Assert.Equal(expected, added["Resources/values.ja.xml"]);
            Assert.Equal("{}"u8.ToArray(), added["a.json"]);
        }

        [Theory]
        [InlineData("[]")]
        [InlineData(/*lang=json,strict*/ "{\"a\":1}")]
        [InlineData("not json")]
        public void AnythingButAnObjectOfStringsIsRefused(string text)
        {
            _ = Assert.ThrowsAny<InvalidDataException>(() => MetadataBundle.Read(Encoding.UTF8.GetBytes(text), (_, _) => { }));
        }
    }
}
