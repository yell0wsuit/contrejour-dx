using System;
using System.Collections.Generic;

using ContreJour.Desktop.Platform.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public sealed class CrashHandlersTests : IDisposable
    {
        public CrashHandlersTests()
        {
            CrashDialog.Enabled = false;
        }

        public void Dispose()
        {
            CrashDialog.Enabled = true;
            CrashHandlers.Configure(null);
            Log.Factory = null;
        }

        [Fact]
        public void AFatalFailureIsLoggedAndTheLogFlushedBeforeTheDialog()
        {
            RecordingProvider recorder = new();
            LoggerFactory factory = new();
            factory.AddProvider(recorder);
            Log.Factory = factory;
            CrashHandlers.Configure(factory);

            CrashHandlers.ReportFatal(new InvalidOperationException("boom"));

            Assert.Contains(recorder.Entries, entry => entry.Level == LogLevel.Critical && entry.Exception?.Message == "boom");
            Assert.True(recorder.Disposed);
            Assert.Same(NullLoggerFactory.Instance, Log.Factory);
        }

        [Fact]
        public void TheDialogNamesTheFirstFailureInsideAnAggregate()
        {
            string text = CrashHandlers.Describe(new AggregateException("No desktop renderer passed.", new InvalidOperationException("Metal failed")));

            Assert.Contains("InvalidOperationException: Metal failed", text, StringComparison.Ordinal);
        }

        [Fact]
        public void AThrowThatIsNotAnExceptionIsStillDescribed()
        {
            Assert.Contains("a thrown string", CrashHandlers.Describe("a thrown string"), StringComparison.Ordinal);
        }

        private sealed class RecordingProvider : ILoggerProvider
        {
            public List<(LogLevel Level, Exception Exception)> Entries { get; } = [];

            public bool Disposed { get; private set; }

            public ILogger CreateLogger(string categoryName)
            {
                return new Recorder(this);
            }

            public void Dispose()
            {
                Disposed = true;
            }

            private sealed class Recorder(RecordingProvider owner) : ILogger
            {
                public IDisposable BeginScope<TState>(TState state) where TState : notnull
                {
                    return null;
                }

                public bool IsEnabled(LogLevel logLevel)
                {
                    return true;
                }

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    owner.Entries.Add((logLevel, exception));
                }
            }
        }
    }
}
