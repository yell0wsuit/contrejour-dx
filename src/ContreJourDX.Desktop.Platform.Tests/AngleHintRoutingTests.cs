using System.Collections.Generic;

using ContreJourDX.Desktop.Platform.Graphics;

using SDL3;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    // Which hints SDL needs to reach ANGLE rather than the system driver, and that none outlive the
    // attempt.
    public class AngleHintRoutingTests
    {
        [Fact]
        public void SdlIsForcedOntoItsEglPath()
        {
            // Without this SDL takes its Windows WGL path, opens the GLES library as if it were the
            // system's opengl32, and fails on wgl entry points it does not have.
            Assert.Equal("1", HintsFor(GlContextProfile.Angle("egl", "gles"))[SDL.Hints.VideoForceEGL]);
        }

        [Fact]
        public void BothLibrariesAreNamedBecauseEachLoaderReadsItsOwn()
        {
            Dictionary<string, string> applied = HintsFor(GlContextProfile.Angle("egl", "gles"));

            Assert.Equal("1", applied[SDL.Hints.OpenGLESDriver]);
            Assert.Equal("egl", applied[SDL.Hints.EGLLibrary]);
            Assert.Equal("gles", applied[SDL.Hints.OpenGLLibrary]);
        }

        [Fact]
        public void TheRetryProfileIsRoutedTheSameWay()
        {
            Dictionary<string, string> applied = HintsFor(GlContextProfile.Angle("egl", "gles").Retry);

            Assert.Equal("1", applied[SDL.Hints.VideoForceEGL]);
            Assert.Equal("egl", applied[SDL.Hints.EGLLibrary]);
            Assert.Equal("gles", applied[SDL.Hints.OpenGLLibrary]);
        }

        // Left behind, these would make the native GL driver tried next open ANGLE's libraries.
        [Fact]
        public void TheHintsAreClearedAgainWhenTheScopeEnds()
        {
            List<string> cleared = [];
            GlHintScope scope = new(_ => null, (_, _) => true, name =>
            {
                cleared.Add(name);
                return true;
            });
            SdlGlDevice.ApplyAngleHints(GlContextProfile.Angle("egl", "gles"), scope);

            scope.Dispose();

            Assert.Equal([SDL.Hints.OpenGLLibrary, SDL.Hints.EGLLibrary, SDL.Hints.OpenGLESDriver, SDL.Hints.VideoForceEGL], cleared);
        }

        private static Dictionary<string, string> HintsFor(GlContextProfile profile)
        {
            Dictionary<string, string> applied = [];
            using GlHintScope scope = new(_ => null, (name, value) =>
            {
                applied[name] = value;
                return true;
            }, _ => true);
            SdlGlDevice.ApplyAngleHints(profile, scope);
            return applied;
        }
    }
}
