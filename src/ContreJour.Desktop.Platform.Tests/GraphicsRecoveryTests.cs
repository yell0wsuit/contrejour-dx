using System;
using System.Collections.Generic;

using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class GraphicsRecoveryTests
    {
        [Theory]
        [InlineData("macos", GraphicsBackendKind.Metal, GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software)]
        [InlineData("macos", GraphicsBackendKind.OpenGL, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Metal, GraphicsBackendKind.Software)]
        [InlineData("linux", GraphicsBackendKind.Software, GraphicsBackendKind.Software, GraphicsBackendKind.Vulkan, GraphicsBackendKind.OpenGL)]
        [InlineData("linux", GraphicsBackendKind.Vulkan, GraphicsBackendKind.Vulkan, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software)]
        [InlineData("windows", GraphicsBackendKind.Vulkan, GraphicsBackendKind.Vulkan, GraphicsBackendKind.Angle, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software)]
        [InlineData("windows", GraphicsBackendKind.OpenGL, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Vulkan, GraphicsBackendKind.Angle, GraphicsBackendKind.Software)]
        public void TheLostRendererIsTriedFirstAndThenTheRest(string platform, GraphicsBackendKind lost, params GraphicsBackendKind[] expected)
        {
            Assert.Equal(expected, new GraphicsRecoveryCoordinator(platform, null).OrderAfter(lost));
        }

        [Fact]
        public void AForcedRendererIsRetriedAndNothingElseIsTried()
        {
            GraphicsRecoveryCoordinator coordinator = new("macos", GraphicsBackendKind.OpenGL);
            GraphicsSelection<Resource> lost = Select(GraphicsBackendKind.OpenGL, []);
            List<GraphicsBackendKind> attempts = [];

            GraphicsRecoveryFailedException failure = Assert.Throws<GraphicsRecoveryFailedException>(() => coordinator.Recover(lost, (kind, _) =>
            {
                attempts.Add(kind);
                throw new InvalidOperationException(kind.ToString());
            }, _ => { }));

            Assert.Equal([GraphicsBackendKind.OpenGL], attempts);
            Assert.Equal([GraphicsBackendKind.OpenGL], failure.Attempted);
            Assert.Equal(0, coordinator.Recoveries);
        }

        [Fact]
        public void TheLostDeviceIsReleasedBeforeAReplacementIsBuilt()
        {
            GraphicsRecoveryCoordinator coordinator = new("macos", null);
            List<string> events = [];
            GraphicsSelection<Resource> lost = Select(GraphicsBackendKind.Metal, events);

            using GraphicsSelection<Resource> replacement = coordinator.Recover(lost, (kind, lifetime) =>
            {
                events.Add("create " + kind);
                return lifetime.Own(new Resource("replacement", events));
            }, _ => { });

            Assert.Equal(["dispose Metal", "create Metal"], events);
            Assert.Equal(1, coordinator.Recoveries);
        }

        [Fact]
        public void ARendererThatWillNotComeBackIsPassedOverForTheNext()
        {
            GraphicsRecoveryCoordinator coordinator = new("macos", null);
            GraphicsSelection<Resource> lost = Select(GraphicsBackendKind.Metal, []);

            using GraphicsSelection<Resource> replacement = coordinator.Recover(lost,
                (kind, lifetime) => kind == GraphicsBackendKind.Metal
                    ? throw new InvalidOperationException("still gone")
                    : lifetime.Own(new Resource(kind.ToString(), [])),
                _ => { });

            Assert.Equal(GraphicsBackendKind.OpenGL, replacement.Kind);
        }

        [Fact]
        public void ATeardownThatFailsStillGetsARunningDevice()
        {
            GraphicsRecoveryCoordinator coordinator = new("macos", null);
            GraphicsSelection<Resource> lost = BackendSelector.Attempt([GraphicsBackendKind.Metal],
                (_, lifetime) => lifetime.Own(new Resource("device", [], new InvalidOperationException("teardown failed"))),
                _ => { });

            using GraphicsSelection<Resource> replacement = coordinator.Recover(lost,
                (kind, lifetime) => lifetime.Own(new Resource("replacement", [])),
                _ => { });

            Assert.Equal(GraphicsBackendKind.Metal, replacement.Kind);
        }

        [Fact]
        public void ATeardownFailureIsReportedWhenNothingComesBack()
        {
            GraphicsRecoveryCoordinator coordinator = new("linux", null);
            InvalidOperationException teardown = new("teardown failed");
            GraphicsSelection<Resource> lost = BackendSelector.Attempt([GraphicsBackendKind.OpenGL],
                (_, lifetime) => lifetime.Own(new Resource("device", [], teardown)),
                _ => { });

            GraphicsRecoveryFailedException failure = Assert.Throws<GraphicsRecoveryFailedException>(() => coordinator.Recover(lost,
                (kind, _) => throw new InvalidOperationException(kind.ToString()), _ => { }));

            Assert.Contains(teardown, ((AggregateException)failure.InnerException).Flatten().InnerExceptions);
        }

        [Fact]
        public void RecoveryIsRefusedAfterFourLossesWithoutAFrame()
        {
            GraphicsRecoveryCoordinator coordinator = new("macos", null);

            for (int i = 0; i < GraphicsRecoveryCoordinator.MaximumFramelessRecoveries; i++)
            {
                Assert.True(coordinator.TryBeginRecovery());
            }

            Assert.False(coordinator.TryBeginRecovery());
        }

        [Fact]
        public void APresentedFrameResetsTheCount()
        {
            GraphicsRecoveryCoordinator coordinator = new("macos", null);
            for (int i = 0; i < GraphicsRecoveryCoordinator.MaximumFramelessRecoveries; i++)
            {
                _ = coordinator.TryBeginRecovery();
            }

            coordinator.FramePresented();

            Assert.True(coordinator.TryBeginRecovery());
        }

        private static GraphicsSelection<Resource> Select(GraphicsBackendKind kind, List<string> events)
        {
            return BackendSelector.Attempt([kind], (k, lifetime) => lifetime.Own(new Resource(k.ToString(), events)), _ => { });
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
