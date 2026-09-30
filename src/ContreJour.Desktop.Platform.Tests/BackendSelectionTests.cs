using System;
using System.Collections.Generic;
using System.Linq;

using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class BackendSelectionTests
    {
        private static readonly GraphicsBackendKind[] AllKinds =
            [GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software];

        [Fact]
        public void MacPrefersMetalThenOpenGLThenSoftware()
        {
            Assert.Equal([GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software], BackendSelector.PreferenceOrder("macos", null));
        }

        [Theory]
        [InlineData("windows")]
        [InlineData("linux")]
        public void OtherPlatformsStartOnOpenGLThenSoftware(string platform)
        {
            Assert.Equal([GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software], BackendSelector.PreferenceOrder(platform, null));
        }

        [Fact]
        public void AForcedRendererIsTheOnlyCandidate()
        {
            Assert.Equal([GraphicsBackendKind.Software], BackendSelector.PreferenceOrder("macos", GraphicsBackendKind.Software));
        }

        [Fact]
        public void AnUnknownPlatformIsRefused()
        {
            _ = Assert.Throws<PlatformNotSupportedException>(() => BackendSelector.PreferenceOrder("amiga", null));
        }

        [Fact]
        public void TheFirstCandidateThatValidatesIsKeptAndTheRejectedOnesReleased()
        {
            List<string> events = [];
            using GraphicsSelection<Resource> selected = BackendSelector.Attempt(AllKinds,
                (kind, lifetime) => lifetime.Own(new Resource(kind.ToString(), events)),
                resource =>
                {
                    events.Add("validate " + resource.Name);
                    if (resource.Name != "OpenGL")
                    {
                        throw new InvalidOperationException(resource.Name);
                    }
                });

            Assert.Equal(GraphicsBackendKind.OpenGL, selected.Kind);
            Assert.Equal(["validate Metal", "dispose Metal", "validate OpenGL"], events);
        }

        [Fact]
        public void PartialInitializationIsUnwoundInReverseBeforeTheNextCandidate()
        {
            List<string> events = [];
            using GraphicsSelection<Resource> selected = BackendSelector.Attempt(AllKinds, (kind, lifetime) =>
            {
                if (kind == GraphicsBackendKind.Metal)
                {
                    _ = lifetime.Own(new Resource("window", events));
                    _ = lifetime.Own(new Resource("device", events));
                    throw new InvalidOperationException("surface creation failed");
                }
                Assert.Equal(["dispose device", "dispose window"], events);
                return lifetime.Own(new Resource("GL", events));
            }, _ => { });

            Assert.Equal("GL", selected.Device.Name);
        }

        [Fact]
        public void EachFailureIsRecordedAgainstTheCandidateThatProducedIt()
        {
            using GraphicsSelection<Resource> selected = BackendSelector.Attempt(AllKinds,
                (kind, lifetime) => lifetime.Own(new Resource(kind.ToString(), [])),
                resource =>
                {
                    if (resource.Name != "Software")
                    {
                        throw new InvalidOperationException(resource.Name);
                    }
                });

            Assert.Equal([GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL], selected.Failures.Select(failure => failure.Kind));
            Assert.Equal("Metal", selected.Failures[0].Failure.Message);
        }

        [Fact]
        public void ACleanupFailureIsBlamedOnTheCandidateBeingCleanedUp()
        {
            using GraphicsSelection<Resource> selected = BackendSelector.Attempt(AllKinds,
                (kind, lifetime) => kind == GraphicsBackendKind.Metal
                    ? lifetime.Own(new Resource("Metal", [], new InvalidOperationException("cleanup failed")))
                    : lifetime.Own(new Resource(kind.ToString(), [])),
                resource =>
                {
                    if (resource.Name == "Metal")
                    {
                        throw new InvalidOperationException("Metal");
                    }
                });

            Assert.All(selected.Failures, failure => Assert.Equal(GraphicsBackendKind.Metal, failure.Kind));
            Assert.Contains(selected.Failures, failure => failure.Failure is AggregateException);
        }

        [Fact]
        public void ExhaustionKeepsEveryFailureAndNamesEachRenderer()
        {
            List<Exception> failures = [];
            AggregateException error = Assert.Throws<AggregateException>(() => BackendSelector.Attempt<Resource>(AllKinds, (kind, _) =>
            {
                InvalidOperationException failure = new("the driver said no");
                failures.Add(failure);
                throw failure;
            }, _ => { }));

            Assert.Equal(failures, error.Flatten().InnerExceptions);
            Assert.Contains("Metal (the driver said no)", error.Message, StringComparison.Ordinal);
            Assert.Contains("OpenGL", error.Message, StringComparison.Ordinal);
            Assert.Contains("Software", error.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void EachCaughtFailureIsAbsolvedOnceItsCandidateIsReleased()
        {
            List<string> events = [];
            using GraphicsSelection<Resource> selected = BackendSelector.Attempt(AllKinds,
                (kind, lifetime) => lifetime.Own(new Resource(kind.ToString(), events)),
                resource =>
                {
                    if (resource.Name != "Software")
                    {
                        throw new InvalidOperationException(resource.Name);
                    }
                },
                () => events.Add("absolve"));

            Assert.Equal(["dispose Metal", "absolve", "dispose OpenGL", "absolve"], events);
        }

        [Fact]
        public void TheSelectionHoldsItsDependenciesUntilDisposed()
        {
            List<string> events = [];
            GraphicsSelection<Resource> selected = BackendSelector.Attempt(AllKinds, (kind, lifetime) =>
            {
                _ = lifetime.Own(new Resource("window", events));
                return lifetime.Own(new Resource("device", events));
            }, _ => { });

            Assert.Empty(events);
            selected.Dispose();
            selected.Dispose();
            Assert.Equal(["dispose device", "dispose window"], events);
        }

        private sealed class Resource(string name, List<string> events, Exception failure = null) : IDisposable
        {
            public string Name { get; } = name;

            public void Dispose()
            {
                events.Add("dispose " + Name);
                if (failure != null)
                {
                    throw failure;
                }
            }
        }
    }
}
