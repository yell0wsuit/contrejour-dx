using System;
using System.IO;

using ContreJourDX.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    // Whether the ANGLE libraries the Windows release ships are on disk.
    public sealed class AngleRuntimeTests : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "cj-angle-" + Guid.NewGuid().ToString("N"));

        public void Dispose()
        {
            try
            {
                Directory.Delete(_root, recursive: true);
            }
            catch (IOException)
            {
                // A temporary folder left behind fails nothing.
            }
        }

        [Fact]
        public void BothLibrariesResolveToAbsolutePathsBesideTheExecutable()
        {
            string baseDirectory = Place("libEGL.dll", "libGLESv2.dll");

            Assert.True(AngleRuntime.TryLocate(baseDirectory, out string egl, out string gles));
            Assert.Equal(Path.Combine(baseDirectory, "angle", "libEGL.dll"), egl);
            Assert.Equal(Path.Combine(baseDirectory, "angle", "libGLESv2.dll"), gles);
            Assert.True(Path.IsPathRooted(egl));
            Assert.True(Path.IsPathRooted(gles));
        }

        [Theory]
        [InlineData("libEGL.dll")]
        [InlineData("libGLESv2.dll")]
        public void HalfAnInstallIsNoInstall(string present)
        {
            string baseDirectory = Place(present);

            Assert.False(AngleRuntime.TryLocate(baseDirectory, out string egl, out string gles));
            Assert.Null(egl);
            Assert.Null(gles);
        }

        [Fact]
        public void AFolderWithoutAngleReportsNothingRatherThanThrowing()
        {
            _ = Directory.CreateDirectory(_root);

            Assert.False(AngleRuntime.TryLocate(_root, out _, out _));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AMissingBaseDirectoryReportsNothing(string baseDirectory)
        {
            Assert.False(AngleRuntime.TryLocate(baseDirectory, out _, out _));
        }

        private string Place(params string[] names)
        {
            string directory = Path.Combine(_root, AngleRuntime.DirectoryName);
            _ = Directory.CreateDirectory(directory);
            foreach (string name in names)
            {
                File.WriteAllText(Path.Combine(directory, name), string.Empty);
            }
            return _root;
        }
    }
}
