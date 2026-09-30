using System;
using System.IO;

using ContreJour.Desktop.Platform.Diagnostics;

using Microsoft.Extensions.Logging;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class LoggingSetupTests
    {
        private const string Header = "Contre Jour\nTest build";

        private static readonly DateTime Stamp = new(2026, 9, 30, 11, 30, 0, DateTimeKind.Local);

        private static string NewRoot()
        {
            return Path.Combine(Path.GetTempPath(), "cj-logs-" + Path.GetRandomFileName());
        }

        private static string LogPath(string root, DateTime stamp)
        {
            return Path.Combine(root, "logs", LoggingSetup.LogFileName(stamp, fallback: false));
        }

        private static void Write(ILoggerFactory factory, LogLevel level, string message)
        {
            factory.CreateLogger("ContreJour.Test").Log(level, default, message, null, static (text, _) => text);
        }

        [Fact]
        public void EachRunWritesItsOwnFileAndLeavesEarlierRunsAlone()
        {
            string root = NewRoot();
            _ = Directory.CreateDirectory(Path.Combine(root, "logs"));
            string earlier = LogPath(root, Stamp.AddDays(-1));
            File.WriteAllText(earlier, "earlier history");
            try
            {
                ILoggerFactory factory = LoggingSetup.Create(root, null, Header, Stamp);
                Write(factory, LogLevel.Information, "new session");
                factory.Dispose();

                Assert.Contains("new session", File.ReadAllText(LogPath(root, Stamp)), StringComparison.Ordinal);
                Assert.Equal("earlier history", File.ReadAllText(earlier));
            }
            finally
            {
                Directory.Delete(root, recursive: true);
            }
        }

        [Fact]
        public void OldRunsArePrunedSoTheDirectoryStaysBounded()
        {
            string root = NewRoot();
            string directory = Path.Combine(root, "logs");
            _ = Directory.CreateDirectory(directory);
            DateTime newest = Stamp.AddHours(-1);
            for (int age = 0; age < 15; age++)
            {
                string path = LogPath(root, newest.AddHours(-age));
                File.WriteAllText(path, "run history");
                File.SetLastWriteTimeUtc(path, DateTime.UtcNow.AddHours(-age));
            }
            try
            {
                LoggingSetup.Create(root, null, Header, Stamp).Dispose();

                // Nine survivors plus this run's own file.
                Assert.Equal(10, Directory.GetFiles(directory, "contrejour-*.log").Length);
                Assert.True(File.Exists(LogPath(root, Stamp)));
                Assert.True(File.Exists(LogPath(root, newest)));
                Assert.False(File.Exists(LogPath(root, newest.AddHours(-9))));
            }
            finally
            {
                Directory.Delete(root, recursive: true);
            }
        }

        [Fact]
        public void EveryLogOpensWithTheHeader()
        {
            string root = NewRoot();
            try
            {
                ILoggerFactory factory = LoggingSetup.Create(root, null, Header, Stamp);
                Write(factory, LogLevel.Information, "after the header");
                factory.Dispose();

                string[] lines = File.ReadAllLines(LogPath(root, Stamp));
                Assert.Equal("Contre Jour", lines[0]);
                Assert.Equal("Test build", lines[1]);
                Assert.Contains(lines, line => line.Contains("after the header", StringComparison.Ordinal));
            }
            finally
            {
                Directory.Delete(root, recursive: true);
            }
        }

        [Fact]
        public void TheHeaderNamesTheBuildAndTheMachine()
        {
            string[] lines = LoggingSetup.ComposeHeader("Contre Jour", "1.2.3").Split(Environment.NewLine);

            Assert.Equal("Contre Jour", lines[0]);
            Assert.EndsWith(" build", lines[1], StringComparison.Ordinal);
            Assert.Equal("Version: 1.2.3", lines[2]);
            Assert.StartsWith("OS: ", lines[3], StringComparison.Ordinal);
            Assert.StartsWith(".NET: ", lines[4], StringComparison.Ordinal);
            Assert.StartsWith("Architecture: ", lines[5], StringComparison.Ordinal);
        }

        [Theory]
        [InlineData(null, LogLevel.Information, true, false)]
        [InlineData(null, LogLevel.Warning, true, true)]
        [InlineData(null, LogLevel.Debug, false, false)]
        [InlineData(LogLevel.Debug, LogLevel.Debug, true, true)]
        [InlineData(LogLevel.Error, LogLevel.Warning, false, false)]
        public void TheFileKeepsInformationAndTheConsoleWarningsUnlessALevelIsAsked(
            LogLevel? requested, LogLevel emitted, bool inFile, bool onConsole)
        {
            string root = NewRoot();
            using StringWriter output = new();
            using StringWriter error = new();
            TextWriter previousOutput = Console.Out;
            TextWriter previousError = Console.Error;
            Console.SetOut(output);
            Console.SetError(error);
            try
            {
                ILoggerFactory factory = LoggingSetup.Create(root, requested, Header, Stamp);
                Write(factory, emitted, "level marker");
                factory.Dispose();

                Assert.Equal(inFile, File.ReadAllText(LogPath(root, Stamp)).Contains("level marker", StringComparison.Ordinal));
                string console = output.ToString() + error.ToString();
                Assert.Equal(onConsole, console.Contains("level marker", StringComparison.Ordinal));
            }
            finally
            {
                Console.SetOut(previousOutput);
                Console.SetError(previousError);
                Directory.Delete(root, recursive: true);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AnUnusableLogDirectoryOrFallbackDoesNotPreventStartup(bool blockFallback)
        {
            string root = NewRoot();
            _ = Directory.CreateDirectory(root);
            try
            {
                if (blockFallback)
                {
                    _ = Directory.CreateDirectory(LogPath(root, Stamp));
                    _ = Directory.CreateDirectory(Path.Combine(root, "logs", LoggingSetup.LogFileName(Stamp, fallback: true)));
                }
                else
                {
                    File.WriteAllText(Path.Combine(root, "logs"), "blocking file");
                }

                using ILoggerFactory factory = LoggingSetup.Create(root, null, Header, Stamp, out string logFile);
                Write(factory, LogLevel.Warning, "console only");
                Assert.Null(logFile);
            }
            finally
            {
                Directory.Delete(root, recursive: true);
            }
        }

        [Fact]
        public void TheLogFolderIsBesideTheSaveData()
        {
            Assert.Equal(Path.Combine("save", "logs"), LoggingSetup.DirectoryFor("save"));
        }

        [Fact]
        public void NoLevelSwitchMeansNoRequestedLevel()
        {
            Assert.Null(LoggingSetup.ParseLevel(["--renderer", "gl"]));
            Assert.Null(LoggingSetup.ParseLevel([]));
        }

        [Theory]
        [InlineData("trace", LogLevel.Trace)]
        [InlineData("debug", LogLevel.Debug)]
        [InlineData("info", LogLevel.Information)]
        [InlineData("warn", LogLevel.Warning)]
        [InlineData("error", LogLevel.Error)]
        [InlineData("DEBUG", LogLevel.Debug)]
        public void ParsesEachLevelInBothSpellings(string value, LogLevel expected)
        {
            Assert.Equal(expected, LoggingSetup.ParseLevel(["--log-level", value]));
            Assert.Equal(expected, LoggingSetup.ParseLevel(["--log-level=" + value]));
        }

        [Fact]
        public void AnUnknownLevelIsRejectedByName()
        {
            ArgumentException failure = Assert.Throws<ArgumentException>(() => LoggingSetup.ParseLevel(["--log-level", "loud"]));

            Assert.Contains("loud", failure.Message, StringComparison.Ordinal);
        }

        [Theory]
        [InlineData("--log-level")]
        [InlineData("--log-level=")]
        public void AMissingLogLevelIsRejected(string argument)
        {
            _ = Assert.Throws<ArgumentException>(() => LoggingSetup.ParseLevel([argument]));
        }

        [Fact]
        public void AnEmptyLogLevelIsRejected()
        {
            _ = Assert.Throws<ArgumentException>(() => LoggingSetup.ParseLevel(["--log-level", ""]));
        }

        [Fact]
        public void RunsInTheSameSecondNeverAppendToAnEarlierRun()
        {
            string root = NewRoot();
            try
            {
                for (int run = 0; run < 3; run++)
                {
                    using ILoggerFactory factory = LoggingSetup.Create(root, null, Header, Stamp);
                    Write(factory, LogLevel.Information, "session " + run);
                }
                string[] files = Directory.GetFiles(Path.Combine(root, "logs"), "*.log");
                Assert.Equal(3, files.Length);
                string first = File.ReadAllText(LogPath(root, Stamp));
                Assert.Contains("session 0", first, StringComparison.Ordinal);
                Assert.DoesNotContain("session 1", first, StringComparison.Ordinal);
                Assert.DoesNotContain("session 2", first, StringComparison.Ordinal);
            }
            finally
            {
                Directory.Delete(root, recursive: true);
            }
        }

    }
}
