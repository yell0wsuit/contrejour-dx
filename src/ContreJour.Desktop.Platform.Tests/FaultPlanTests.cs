using System;

using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class FaultPlanTests
    {
        [Fact]
        public void NoPlanNeverThrows()
        {
            Action<string> fault = FaultPlan.None.For(GraphicsBackendKind.OpenGL);

            fault("after-device");
            fault("before-surface");
            fault("after-surface");
        }

        [Fact]
        public void AFaultFiresAtItsPointOnly()
        {
            Action<string> fault = FaultPlan.Parse(["metal:after-surface"]).For(GraphicsBackendKind.Metal);

            fault("after-device");
            fault("before-surface");
            InvalidOperationException failure = Assert.Throws<InvalidOperationException>(() => fault("after-surface"));
            Assert.Contains("Metal:after-surface#1", failure.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void AFaultFiresOnce()
        {
            Action<string> fault = FaultPlan.Parse(["metal:before-surface"]).For(GraphicsBackendKind.Metal);

            _ = Assert.Throws<InvalidOperationException>(() => fault("before-surface"));
            fault("before-surface");
        }

        [Fact]
        public void ByDefaultOnlyTheFirstDeviceOfAKindFails()
        {
            FaultPlan plan = FaultPlan.Parse(["gl:after-device"]);

            _ = Assert.Throws<InvalidOperationException>(() => plan.For(GraphicsBackendKind.OpenGL)("after-device"));
            plan.For(GraphicsBackendKind.OpenGL)("after-device");
        }

        [Fact]
        public void AnInstanceNumberTargetsALaterDevice()
        {
            FaultPlan plan = FaultPlan.Parse(["metal:after-device#2"]);

            plan.For(GraphicsBackendKind.Metal)("after-device");
            _ = Assert.Throws<InvalidOperationException>(() => plan.For(GraphicsBackendKind.Metal)("after-device"));
        }

        [Fact]
        public void OtherKindsAreUnaffected()
        {
            FaultPlan plan = FaultPlan.Parse(["metal:after-device"]);

            plan.For(GraphicsBackendKind.OpenGL)("after-device");
            plan.For(GraphicsBackendKind.Software)("after-device");
        }

        [Theory]
        [InlineData("")]
        [InlineData("metal")]
        [InlineData("vulkan:after-device")]
        [InlineData("metal:nowhere")]
        [InlineData("metal:after-device#0")]
        [InlineData("metal:after-device#x")]
        public void MalformedFaultsAreRejected(string spec)
        {
            _ = Assert.Throws<ArgumentException>(() => FaultPlan.Parse([spec]));
        }
    }
}
