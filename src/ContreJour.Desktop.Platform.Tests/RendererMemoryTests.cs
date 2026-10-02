using System;
using System.IO;

using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    // What a launch that never came back tells the next one.
    public sealed class RendererMemoryTests : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "cj-renderer-" + Guid.NewGuid().ToString("N"));

        private string StatePath => Path.Combine(_root, RendererMemory.FileName);

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
        public void AFirstLaunchBlamesNothing()
        {
            RendererMemory memory = new(StatePath);

            Assert.Null(memory.Blamed);
            Assert.Equal([GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL], memory.Filter([GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL]));
        }

        [Fact]
        public void ARendererThatKilledTheLastLaunchIsNotTriedAgain()
        {
            new RendererMemory(StatePath).BeginAttempt(GraphicsBackendKind.Metal);

            RendererMemory next = new(StatePath);

            Assert.Equal(GraphicsBackendKind.Metal, next.Blamed);
            Assert.Equal([GraphicsBackendKind.OpenGL], next.Filter([GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL]));
        }

        [Theory]
        [InlineData(GraphicsBackendKind.Vulkan)]
        [InlineData(GraphicsBackendKind.Angle)]
        public void TheNewRenderersCanBeBlamed(GraphicsBackendKind kind)
        {
            new RendererMemory(StatePath).BeginAttempt(kind);

            Assert.Equal(kind, new RendererMemory(StatePath).Blamed);
        }

        [Fact]
        public void ARendererThatDrewAFrameIsForgiven()
        {
            RendererMemory memory = new(StatePath);
            memory.BeginAttempt(GraphicsBackendKind.Metal);
            memory.RecordSuccess();

            Assert.Null(new RendererMemory(StatePath).Blamed);
        }

        [Fact]
        public void ACandidateThatFailedAndCameBackIsNotBlamed()
        {
            RendererMemory memory = new(StatePath);
            memory.BeginAttempt(GraphicsBackendKind.OpenGL);

            memory.Absolve();

            Assert.Null(new RendererMemory(StatePath).Blamed);
        }

        [Fact]
        public void AForcedRendererIsStillTried()
        {
            new RendererMemory(StatePath).BeginAttempt(GraphicsBackendKind.Metal);

            Assert.Equal([GraphicsBackendKind.Metal], new RendererMemory(StatePath).Filter([GraphicsBackendKind.Metal]));
        }

        [Fact]
        public void AnUnreadableMarkerIsTreatedAsNone()
        {
            _ = Directory.CreateDirectory(_root);
            File.WriteAllText(StatePath, "not-a-renderer");

            Assert.Null(new RendererMemory(StatePath).Blamed);
        }

        [Fact]
        public void ADirectoryThatCannotBeWrittenDoesNotStopTheGame()
        {
            RendererMemory memory = new(Path.Combine(_root, "\0bad", RendererMemory.FileName));

            memory.BeginAttempt(GraphicsBackendKind.Metal);
            memory.RecordSuccess();
        }

        [Fact]
        public void EveryCandidateFailingCleanlyLeavesTheNextLaunchTheFullOrder()
        {
            RendererMemory memory = new(StatePath);
            GraphicsBackendKind[] order = [GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software];

            _ = Assert.Throws<AggregateException>(() => BackendSelector.Attempt<Blank>(memory.Filter(order), (kind, _) =>
            {
                memory.BeginAttempt(kind);
                throw new InvalidOperationException(kind.ToString());
            }, _ => { }, memory.Absolve));

            RendererMemory next = new(StatePath);
            Assert.Null(next.Blamed);
            Assert.Equal(order, next.Filter(order));
        }

        private sealed class Blank : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
