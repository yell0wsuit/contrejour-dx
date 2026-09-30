using System;
using System.IO;

using ContreJour.Desktop.Platform.Diagnostics;

using Microsoft.Extensions.Logging;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    // A log is written to be sent to someone, and almost every path in it runs through the home
    // folder, whose name says who sent it.
    public class LogEntryFormatTests
    {
        private static string Account => Environment.UserName;

        [Fact]
        public void TheAccountNameIsTakenOutOfAPath()
        {
            string redacted = LogEntryFormat.Redact($"/Users/{Account}/Documents/ContreJourDX_SaveData");

            Assert.DoesNotContain(Account, redacted, StringComparison.Ordinal);
            Assert.Contains(LogEntryFormat.RedactedUserName, redacted, StringComparison.Ordinal);
        }

        [Fact]
        public void TheAccountNameIsTakenOutWhateverCaseItArrivesIn()
        {
            string redacted = LogEntryFormat.Redact($@"C:\USERS\{Account.ToUpperInvariant()}\save");

            Assert.DoesNotContain(Account, redacted, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void AComposedEntryIsRedactedThroughItsException()
        {
            Exception failure = new IOException($"cannot open /Users/{Account}/save.json");

            string line = LogEntryFormat.Compose(LogLevel.Error, "ContreJour.Host", $"saving to /Users/{Account}", failure);

            Assert.DoesNotContain(Account, line, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void TextWithoutTheAccountNameIsLeftAlone()
        {
            const string ordinary = "Renderer Metal, audio on";

            Assert.Equal(ordinary, LogEntryFormat.Redact(ordinary));
        }

        [Fact]
        public void ComposeBracketsTheLevelAndPutsTheExceptionOnItsOwnLine()
        {
            string line = LogEntryFormat.Compose(LogLevel.Warning, "ContreJour.Test", "message", new InvalidOperationException("boom"));

            string[] lines = line.Split(Environment.NewLine);
            Assert.EndsWith(" [Warning] ContreJour.Test message", lines[0], StringComparison.Ordinal);
            Assert.StartsWith("System.InvalidOperationException: boom", lines[1], StringComparison.Ordinal);
        }

        [Fact]
        public void TheConsoleColorsTheLevelOnlyWhenAsked()
        {
            string colored = WriteConsoleEntry(LogLevel.Warning, color: true);
            string plain = WriteConsoleEntry(LogLevel.Warning, color: false);

            Assert.Contains("\u001b[1m\u001b[33m[Warning]\u001b[39m\u001b[49m\u001b[22m ContreJour.Test message", colored, StringComparison.Ordinal);
            Assert.Contains("[Warning] ContreJour.Test message", plain, StringComparison.Ordinal);
            Assert.DoesNotContain("\u001b", plain, StringComparison.Ordinal);
        }

        private static string WriteConsoleEntry(LogLevel level, bool color)
        {
            using StringWriter written = new();
            new RedactingConsoleFormatter(_ => color).Write(
                new Microsoft.Extensions.Logging.Abstractions.LogEntry<string>(level, "ContreJour.Test", default, "state", null, (_, _) => "message"),
                null,
                written);
            return written.ToString();
        }
    }
}
