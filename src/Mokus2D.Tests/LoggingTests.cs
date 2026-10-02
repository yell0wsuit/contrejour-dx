using Mokus2D.Diagnostics;

using Xunit;

namespace Mokus2D.Tests
{
    public sealed class LoggingTests
    {
        [Fact]
        public void FactoryReplacementRetiresCachedLoggersAndNullRestoresSilence()
        {
            using RecordingLoggerFactory first = new();
            using RecordingLoggerFactory second = new();
            try
            {
                Log.Factory = first;
                Assert.Same(Log.For(LogCategories.Content), Log.For(LogCategories.Content));
                Log.Factory = second;
                Assert.NotSame(first.CreateLogger(LogCategories.Content), Log.For(LogCategories.Content));
                Log.For(LogCategories.Content).Log(Microsoft.Extensions.Logging.LogLevel.Warning, default, "new factory", null, static (text, _) => text);
                Assert.Empty(first.Entries);
                _ = Assert.Single(second.Entries);
                Log.Factory = null;
                Assert.False(Log.For(LogCategories.Content).IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical));
            }
            finally { Log.Factory = null; }
        }
    }
}
