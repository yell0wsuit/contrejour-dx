using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;

using Xunit;

namespace ContreJourDX.Browser.Platform.Tests
{
    public sealed class BrowserLoggingTests
    {
        [Fact]
        public void FiltersDebugAndKeepsFormattedErrorsWithExceptions()
        {
            List<(string Line, bool Urgent)> stored = [];
            using StringWriter output = new();
            using StringWriter error = new();
            using BrowserLogFactory factory = new((line, urgent) => stored.Add((line, urgent)), output, error);
            ILogger logger = factory.CreateLogger("test.category");
            logger.Log(LogLevel.Debug, default, "hidden", null, static (text, _) => text);
            logger.Log(LogLevel.Information, default, "visible", null, static (text, _) => text);
            logger.Log(LogLevel.Error, default, "failed", new IOException("test failure"), static (text, _) => text);
            Assert.Equal(2, stored.Count);
            Assert.False(stored[0].Urgent);
            Assert.True(stored[1].Urgent);
            Assert.Contains("[Information] test.category visible", output.ToString(), StringComparison.Ordinal);
            Assert.Contains("System.IO.IOException: test failure", error.ToString(), StringComparison.Ordinal);
            Assert.NotNull(logger.BeginScope("scope"));
        }

        [Fact]
        public void PersistenceFailureLeavesConsoleLoggingAvailable()
        {
            using StringWriter output = new();
            using StringWriter error = new();
            using BrowserLogFactory factory = new((_, _) => throw new IOException("storage disabled"), output, error);
            factory.CreateLogger("test").Log(LogLevel.Warning, default, "still running", null, static (text, _) => text);
            Assert.Contains("still running", output.ToString(), StringComparison.Ordinal);
        }
    }
}
