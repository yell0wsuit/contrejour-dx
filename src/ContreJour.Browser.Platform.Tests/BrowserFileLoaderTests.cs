using System.IO;

using Xunit;

namespace ContreJour.Browser.Platform.Tests
{
    public class BrowserFileLoaderTests
    {
        [Fact]
        public void OpensAddedBytes()
        {
            BrowserFileLoader files = new();
            files.Add("Assets/Content/fonts/fonts.json", [1, 2, 3]);

            using Stream stream = files.OpenFile("Assets/Content/fonts/fonts.json");
            using MemoryStream copy = new();
            stream.CopyTo(copy);

            Assert.Equal(new byte[] { 1, 2, 3 }, copy.ToArray());
        }

        [Fact]
        public void LookupIgnoresCaseAndSlashDirection()
        {
            BrowserFileLoader files = new();
            files.Add("Assets/Content/Graphics/menu/menu.webp", [7]);

            Assert.True(files.Contains(@"assets\content\graphics\Menu\MENU.webp"));
            using Stream stream = files.OpenFile("./Assets/Content/Graphics/menu/menu.webp");
            Assert.Equal(7, stream.ReadByte());
        }

        [Fact]
        public void MissingFilesThrowFileNotFound()
        {
            BrowserFileLoader files = new();

            FileNotFoundException missing = Assert.Throws<FileNotFoundException>(() => files.OpenFile("Resources/values.xx.xml"));
            Assert.Equal("Resources/values.xx.xml", missing.FileName);
        }

        [Fact]
        public void StreamsAreReadOnly()
        {
            BrowserFileLoader files = new();
            files.Add("a.json", [1]);

            using Stream stream = files.OpenFile("a.json");

            Assert.False(stream.CanWrite);
        }
    }
}
