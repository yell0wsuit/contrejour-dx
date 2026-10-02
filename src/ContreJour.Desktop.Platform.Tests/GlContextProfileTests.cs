using System;
using System.Collections.Generic;

using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    // Which context each GL renderer asks SDL for, and when a refusal earns a second try.
    public class GlContextProfileTests
    {
        [Fact]
        public void TheDesktopProfileAsksTheSystemDriverForCoreThreeTwo()
        {
            GlContextProfile profile = GlContextProfile.DesktopCore;

            Assert.Equal(GlContextProfile.CoreMask, profile.ProfileMask);
            Assert.Equal(3, profile.Major);
            Assert.Equal(2, profile.Minor);
            Assert.False(profile.UsesAngle);
            Assert.Null(profile.EglLibrary);
            Assert.Null(profile.GlesLibrary);
            Assert.Null(profile.Retry);
        }

        [Fact]
        public void TheAngleProfileAsksForEsThreeFromItsOwnLibraries()
        {
            GlContextProfile profile = GlContextProfile.Angle("/opt/angle/libEGL.dll", "/opt/angle/libGLESv2.dll");

            Assert.Equal(GlContextProfile.EsMask, profile.ProfileMask);
            Assert.Equal(3, profile.Major);
            Assert.Equal(0, profile.Minor);
            Assert.True(profile.UsesAngle);
            Assert.Equal("/opt/angle/libEGL.dll", profile.EglLibrary);
            Assert.Equal("/opt/angle/libGLESv2.dll", profile.GlesLibrary);
        }

        [Fact]
        public void TheAngleProfileRetriesOnceAtEsTwoWithTheSameLibraries()
        {
            GlContextProfile retry = GlContextProfile.Angle("egl", "gles").Retry;

            Assert.NotNull(retry);
            Assert.Equal(GlContextProfile.EsMask, retry.ProfileMask);
            Assert.Equal(2, retry.Major);
            Assert.Equal(0, retry.Minor);
            Assert.Equal("egl", retry.EglLibrary);
            Assert.Equal("gles", retry.GlesLibrary);
            Assert.Null(retry.Retry);
        }

        [Fact]
        public void ARefusedEsThreeContextIsRetriedOnceAtEsTwo()
        {
            List<int> asked = [];

            string started = GlContextProfile.Angle("egl", "gles").Start(profile =>
            {
                asked.Add(profile.Major);
                return profile.Major == 3 ? throw new GlContextRefusedException("ES 3.0 refused") : "ES 2.0";
            });

            Assert.Equal("ES 2.0", started);
            Assert.Equal([3, 2], asked);
        }

        // An injected fault is an InvalidOperationException too; retrying on it would bring ANGLE up
        // at ES 2.0 when the run asked for ANGLE to fail.
        [Fact]
        public void AnyOtherFailureIsNotRetried()
        {
            int attempts = 0;

            _ = Assert.Throws<InvalidOperationException>(() => GlContextProfile.Angle("egl", "gles").Start<string>(_ =>
            {
                attempts++;
                throw new InvalidOperationException("Injected failure at Angle:after-device#1.");
            }));

            Assert.Equal(1, attempts);
        }

        [Fact]
        public void ARefusedDesktopContextIsNotRetried()
        {
            int attempts = 0;

            _ = Assert.Throws<GlContextRefusedException>(() => GlContextProfile.DesktopCore.Start<string>(_ =>
            {
                attempts++;
                throw new GlContextRefusedException("core 3.2 refused");
            }));

            Assert.Equal(1, attempts);
        }

        [Fact]
        public void ARefusedRetryFailsTheCandidate()
        {
            int attempts = 0;

            _ = Assert.Throws<GlContextRefusedException>(() => GlContextProfile.Angle("egl", "gles").Start<string>(_ =>
            {
                attempts++;
                throw new GlContextRefusedException("refused");
            }));

            Assert.Equal(2, attempts);
        }
    }
}
