using System;
using System.Globalization;

using ContreJourDX.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    // Which Vulkan results the host can recover from by replacing the device.
    public class VulkanApiTests
    {
        [Theory]
        [InlineData(Vk.ErrorDeviceLost)]
        [InlineData(Vk.ErrorSurfaceLostKhr)]
        [InlineData(Vk.ErrorFullScreenExclusiveModeLostExt)]
        public void ALostDeviceOrSurfaceIsSomethingTheHostRecoversFrom(int result)
        {
            GraphicsDeviceLostException lost = Assert.Throws<GraphicsDeviceLostException>(() => VulkanApi.Check(result, "vkQueuePresentKHR"));

            Assert.Contains("vkQueuePresentKHR", lost.Message, StringComparison.Ordinal);
            Assert.Contains(result.ToString(CultureInfo.InvariantCulture), lost.Message, StringComparison.Ordinal);
        }

        // An out-of-date swapchain (a resize, F11) is rebuilt by the device itself; as a loss it would
        // replace the whole device on every resize.
        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(Vk.ErrorOutOfDateKhr)]
        public void AnyOtherFailureIsAnOrdinaryFailure(int result)
        {
            _ = Assert.Throws<InvalidOperationException>(() => VulkanApi.Check(result, "vkQueueSubmit"));
            Assert.False(VulkanApi.IsDeviceLost(result));
        }

        [Fact]
        public void ASucceedingCallReportsNothing()
        {
            VulkanApi.Check(Vk.Success, "vkQueueSubmit");
        }
    }
}
