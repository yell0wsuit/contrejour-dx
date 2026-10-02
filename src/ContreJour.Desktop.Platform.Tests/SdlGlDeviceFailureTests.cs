using System;

using ContreJour.Desktop.Platform.Graphics;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    // How the GL device reports what SDL refused.
    public class SdlGlDeviceFailureTests
    {
        // On ANGLE, SDL picks the EGL config while it makes the window; an ES 3.0 request on Direct3D
        // feature level 10_0 finds none there, before any context is asked for.
        [Fact]
        public void AWindowAngleWillNotMakeIsARefusalTheRetryCanAnswer()
        {
            InvalidOperationException failure = new("Couldn't find matching EGL config");

            GlContextRefusedException refused = SdlGlDevice.WindowRefused(GlContextProfile.Angle("egl", "gles"), failure);

            Assert.Same(failure, refused.InnerException);
            Assert.Contains("ES 3.0", refused.Message, StringComparison.Ordinal);
            Assert.Contains("Couldn't find matching EGL config", refused.Message, StringComparison.Ordinal);
        }

        // A swap that fails is what a driver reset looks like on GL, so the host replaces the device.
        [Fact]
        public void AFailedSwapIsALostDevice()
        {
            GraphicsDeviceLostException lost = SdlGlDevice.SwapFailed("EGL_CONTEXT_LOST");

            Assert.Contains("EGL_CONTEXT_LOST", lost.Message, StringComparison.Ordinal);
        }
    }
}
