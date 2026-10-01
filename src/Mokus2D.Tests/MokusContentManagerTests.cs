using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;

using Mokus2D.Content;
using Mokus2D.Diagnostics;
using Mokus2D.FileSystem;

using Xunit;

namespace Mokus2D.Tests
{
    public class MokusContentManagerTests
    {
        private sealed class MissingFiles : IFileLoader
        {
            public List<string> Opened { get; } = [];

            public Stream OpenFile(string path)
            {
                Opened.Add(path);
                throw new FileNotFoundException(path);
            }
        }

        [Fact]
        public void TexturesAreOpenedWithTheHostImageExtension()
        {
            using RecordingLoggerFactory factory = new();
            Log.Factory = factory;
            MissingFiles files = new();
            using MokusContentManager content = new(files, null, new ContentFormats(".webp", ".ogg", ".ogg"))
            {
                RootDirectory = "Assets/Content",
            };

            _ = Assert.Throws<FileNotFoundException>(() => content.Load("Graphics/textures/tail.x0.5.png"));
            _ = Assert.Throws<FileNotFoundException>(() => content.Load("Graphics/menu/menu"));

            Assert.Equal(
                [Path.Combine("Assets/Content", "Graphics/textures/tail.x0.5.webp"), Path.Combine("Assets/Content", "Graphics/menu/menu.webp")],
                files.Opened);
            Log.Factory = null;
            Assert.Equal(2, factory.Entries.Count);
            Assert.All(factory.Entries, entry =>
            {
                Assert.Equal(LogCategories.Content, entry.Category);
                Assert.Equal(LogLevel.Error, entry.Level);
                _ = Assert.IsType<FileNotFoundException>(entry.Exception);
            });
        }

        [Fact]
        public void DesktopFormatsArePngWavFlac()
        {
            Assert.Equal(new ContentFormats(".png", ".wav", ".flac"), ContentFormats.Desktop);
        }
    }
}
