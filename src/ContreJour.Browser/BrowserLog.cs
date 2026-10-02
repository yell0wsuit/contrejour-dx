using System;

using Microsoft.Extensions.Logging;

namespace ContreJour.Browser
{
    internal static partial class BrowserLog
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "cj-audio: none in the content; running silent")]
        public static partial void NoAudio(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "cj-boot-complete: language {Language}, {Images} images, {Fonts} fonts, {Sounds} sounds, {Songs} songs")]
        public static partial void BootComplete(ILogger logger, string language, int images, int fonts, int sounds, int songs);

        [LoggerMessage(Level = LogLevel.Error, Message = "cj-frame-error")]
        public static partial void FrameFailed(ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "cj-frame-request-error")]
        public static partial void FrameRequestFailed(ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Warning, Message = "cj-host-events-dropped: total={Dropped}")]
        public static partial void EventsDropped(ILogger logger, int dropped);

        [LoggerMessage(Level = LogLevel.Information, Message = "cj-game-started: canvas {Width}x{Height}")]
        public static partial void GameStarted(ILogger logger, int width, int height);

        [LoggerMessage(Level = LogLevel.Information, Message = "cj-running: {Frames} frames")]
        public static partial void Running(ILogger logger, int frames);

        [LoggerMessage(Level = LogLevel.Error, Message = "cj-context-lost: reload required")]
        public static partial void ContextLost(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Preloading {Kind}: {Count} assets")]
        public static partial void Preloading(ILogger logger, string kind, int count);

        [LoggerMessage(Level = LogLevel.Error, Message = "Could not fetch {Url}")]
        public static partial void FetchFailed(ILogger logger, string url, Exception exception);
    }
}
