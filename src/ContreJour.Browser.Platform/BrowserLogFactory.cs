using System;
using System.IO;

using Microsoft.Extensions.Logging;

using Mokus2D.Diagnostics;

namespace ContreJour.Browser.Platform
{
    // An abstractions-only factory, like CutTheRopeDX's browser provider. The single-threaded
    // host writes straight to the JS batch buffer; no worker queue or DI container is needed.
    public sealed class BrowserLogFactory(Action<string, bool> append, TextWriter output, TextWriter error) : ILoggerFactory
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new BrowserLogger(categoryName, append, output, error);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotSupportedException("The browser factory owns its single log provider.");
        }

        public void Dispose()
        {
        }

        private sealed class BrowserLogger(string category, Action<string, bool> append, TextWriter output, TextWriter error) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) where TState : notnull
            {
                return EmptyScope.Instance;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel is >= LogLevel.Information and < LogLevel.None;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }
                string line = LogEntryFormat.Compose(logLevel, category, formatter(state, exception), exception);
                (logLevel >= LogLevel.Error ? error : output).WriteLine(line);
                try
                {
                    append(line, logLevel >= LogLevel.Warning);
                }
                catch (Exception)
                {
                    // A refused JS storage call must not interrupt boot or escape a native frame callback.
                }
            }
        }

        private sealed class EmptyScope : IDisposable
        {
            public static EmptyScope Instance { get; } = new();

            public void Dispose()
            {
            }
        }
    }
}
