using System;

using Microsoft.Extensions.Logging;

namespace Mokus2D.Diagnostics
{
    internal static partial class EngineLog
    {
        [LoggerMessage(Level = LogLevel.Error, Message = "Could not load texture {Asset}")]
        public static partial void TextureFailed(ILogger logger, string asset, Exception exception);

        [LoggerMessage(Level = LogLevel.Information, Message = "Loaded audio {File}")]
        public static partial void AudioLoaded(ILogger logger, string file);

        [LoggerMessage(Level = LogLevel.Error, Message = "Could not load audio {File}")]
        public static partial void AudioFailed(ILogger logger, string file, Exception exception);

        [LoggerMessage(Level = LogLevel.Information, Message = "Application initialized: {Width}x{Height}")]
        public static partial void Initialized(ILogger logger, int width, int height);

        [LoggerMessage(Level = LogLevel.Information, Message = "Application disposed")]
        public static partial void Disposed(ILogger logger);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Resource {Resource} at scale {Suffix} failed; trying default scale")]
        public static partial void ResourceFallback(ILogger logger, string resource, string suffix, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Could not load resource {Resource}")]
        public static partial void ResourceFailed(ILogger logger, string resource, Exception exception);
    }
}
