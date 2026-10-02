using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Mokus2D.Tests
{
    public sealed class RecordingLoggerFactory : ILoggerFactory
    {
        public List<(string Category, LogLevel Level, string Message, Exception Exception)> Entries { get; } = [];

        public ILogger CreateLogger(string categoryName)
        {
            return new Recorder(this, categoryName);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotSupportedException();
        }

        public void Dispose() { }

        private sealed class Recorder(RecordingLoggerFactory owner, string category) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) where TState : notnull
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel != LogLevel.None;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                owner.Entries.Add((category, logLevel, formatter(state, exception), exception));
            }
        }
    }
}
