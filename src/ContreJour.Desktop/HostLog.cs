using Microsoft.Extensions.Logging;

namespace ContreJour.Desktop
{
    internal static partial class HostLog
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "Audio unavailable: {Reason}")]
        public static partial void AudioUnavailable(ILogger logger, string reason);

        [LoggerMessage(Level = LogLevel.Information, Message = "Quitting after {Frames} frames, as asked")]
        public static partial void QuittingAfterFrames(ILogger logger, int frames);
    }
}
