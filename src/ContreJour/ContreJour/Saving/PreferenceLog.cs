using System;

using Microsoft.Extensions.Logging;

namespace ContreJour.Saving
{
    internal static partial class PreferenceLog
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "Could not load {File}; starting with empty preferences")]
        public static partial void LoadFailed(ILogger logger, string file, Exception exception);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Save failed on attempt {Attempt}; will retry")]
        public static partial void SaveFailed(ILogger logger, int attempt, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Save abandoned after {Attempts} attempts; changes remain dirty")]
        public static partial void SaveAbandoned(ILogger logger, int attempts, Exception exception);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Saved {File}")]
        public static partial void Saved(ILogger logger, string file);

        [LoggerMessage(Level = LogLevel.Information, Message = "Preferences loaded from {Store}")]
        public static partial void Loaded(ILogger logger, string store);
    }
}
