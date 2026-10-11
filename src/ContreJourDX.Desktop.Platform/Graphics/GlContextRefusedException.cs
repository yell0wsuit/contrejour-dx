using System;

namespace ContreJourDX.Desktop.Platform.Graphics
{
    // SDL would not create the GL context a profile asked for: the driver, or ANGLE on this hardware,
    // does not offer that version. The one failure a lower profile can answer.
    public sealed class GlContextRefusedException : InvalidOperationException
    {
        public GlContextRefusedException()
        {
        }

        public GlContextRefusedException(string message) : base(message)
        {
        }

        public GlContextRefusedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
