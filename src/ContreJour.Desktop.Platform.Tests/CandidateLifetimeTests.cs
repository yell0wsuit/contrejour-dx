using System;
using System.Collections.Generic;

using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class CandidateLifetimeTests
    {
        [Fact]
        public void ResourcesAreReleasedInReverseOrder()
        {
            List<string> events = [];
            CandidateLifetime lifetime = new();
            _ = lifetime.Own(new Resource("window", events));
            _ = lifetime.Own(new Resource("context", events));

            lifetime.Dispose();

            Assert.Equal(["dispose context", "dispose window"], events);
        }

        [Fact]
        public void DisposingTwiceReleasesOnce()
        {
            List<string> events = [];
            CandidateLifetime lifetime = new();
            _ = lifetime.Own(new Resource("device", events));

            lifetime.Dispose();
            lifetime.Dispose();

            Assert.Equal(["dispose device"], events);
        }

        [Fact]
        public void EveryResourceIsReleasedEvenWhenOneThrows()
        {
            List<string> events = [];
            InvalidOperationException failure = new("context cleanup failed");
            CandidateLifetime lifetime = new();
            _ = lifetime.Own(new Resource("window", events));
            _ = lifetime.Own(new Resource("context", events, failure));

            AggregateException error = Assert.Throws<AggregateException>(lifetime.Dispose);

            Assert.Equal(["dispose context", "dispose window"], events);
            Assert.Contains(failure, error.InnerExceptions);
        }

        [Fact]
        public void NothingCanBeOwnedAfterDisposal()
        {
            CandidateLifetime lifetime = new();
            lifetime.Dispose();

            _ = Assert.Throws<ObjectDisposedException>(() => lifetime.Own(new Resource("late", [])));
        }

        private sealed class Resource(string name, List<string> events, Exception failure = null) : IDisposable
        {
            public void Dispose()
            {
                events.Add("dispose " + name);
                if (failure != null)
                {
                    throw failure;
                }
            }
        }
    }
}
