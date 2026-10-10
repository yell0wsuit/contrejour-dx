using System;

namespace ContreJourDX.Desktop.Platform.Graphics
{
    // The graphics device stopped being usable while the game was running: the host replaces it.
    public sealed class GraphicsDeviceLostException : Exception
    {
        public GraphicsDeviceLostException()
        {
        }

        public GraphicsDeviceLostException(string message) : base(message)
        {
        }

        public GraphicsDeviceLostException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
