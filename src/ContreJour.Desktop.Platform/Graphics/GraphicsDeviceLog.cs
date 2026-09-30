using Microsoft.Extensions.Logging;

namespace ContreJour.Desktop.Platform.Graphics
{
    internal static partial class GraphicsDeviceLog
    {
        // One shape for every backend, so the adapter line reads the same everywhere.
        [LoggerMessage(Level = LogLevel.Information, Message = "Adapter {Backend}: {Name}, {Version}")]
        public static partial void Adapter(ILogger logger, GraphicsBackendKind backend, string name, string version);
    }
}
