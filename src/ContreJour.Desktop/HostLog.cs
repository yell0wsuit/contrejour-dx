using ContreJour.Desktop.Platform.Graphics;

using Microsoft.Extensions.Logging;

namespace ContreJour.Desktop
{
    internal static partial class HostLog
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "Audio unavailable: {Reason}")]
        public static partial void AudioUnavailable(ILogger logger, string reason);

        [LoggerMessage(Level = LogLevel.Information, Message = "Quitting after {Frames} frames, as asked")]
        public static partial void QuittingAfterFrames(ILogger logger, int frames);
        [LoggerMessage(Level = LogLevel.Information, Message = "Renderer {Renderer}, audio {AudioState}")]
        public static partial void Renderer(ILogger logger, GraphicsBackendKind renderer, string audioState);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Rejected {Renderer}: {Reason}")]
        public static partial void RejectedRenderer(ILogger logger, GraphicsBackendKind renderer, string reason);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Skipping {Renderer}: the last launch did not survive starting it")]
        public static partial void SkippingBlamedRenderer(ILogger logger, GraphicsBackendKind renderer);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Window focus {Active}")]
        public static partial void FocusChanged(ILogger logger, bool active);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Graphics device lost: {Reason}")]
        public static partial void DeviceLost(ILogger logger, string reason);

        [LoggerMessage(Level = LogLevel.Information, Message = "Recovered on {Renderer} at frame {Frame} (recovery {Recoveries})")]
        public static partial void Recovered(ILogger logger, GraphicsBackendKind renderer, int frame, int recoveries);

        [LoggerMessage(Level = LogLevel.Error, Message = "Abandoning: {Reason}")]
        public static partial void Abandoning(ILogger logger, string reason);
    }
}
