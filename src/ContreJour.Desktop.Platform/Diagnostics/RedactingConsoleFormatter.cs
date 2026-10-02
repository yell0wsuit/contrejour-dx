using System;
using System.IO;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

using Mokus2D.Diagnostics;

namespace ContreJour.Desktop.Platform.Diagnostics
{
    // Writes console entries in the file's shape, account name removed: a bug report is usually a
    // paste of the terminal. The level is bold and colored only on a terminal and without NO_COLOR.
    public sealed class RedactingConsoleFormatter : ConsoleFormatter
    {
        public const string FormatterName = "contrejour";

        // Error and worse go to standard error.
        public const LogLevel StandardErrorThreshold = LogLevel.Error;

        private const string Escape = "\u001b[";

        private static readonly bool ColorDisabled = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR"));

        private readonly Func<LogLevel, bool> _colorFor;

        public RedactingConsoleFormatter() : this(ShouldColor)
        {
        }

        internal RedactingConsoleFormatter(Func<LogLevel, bool> colorFor) : base(FormatterName)
        {
            _colorFor = colorFor;
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            ArgumentNullException.ThrowIfNull(textWriter);
            string message = logEntry.Formatter is null
                ? logEntry.State?.ToString()
                : logEntry.Formatter(logEntry.State, logEntry.Exception);
            string line = LogEntryFormat.Compose(logEntry.LogLevel, logEntry.Category, message, logEntry.Exception);
            if (_colorFor(logEntry.LogLevel))
            {
                line = ColorLevel(line, logEntry.LogLevel);
            }
            textWriter.WriteLine(line);
        }

        private static string ColorLevel(string line, LogLevel level)
        {
            string token = "[" + level + "]";
            int at = line.IndexOf(token, StringComparison.Ordinal);
            if (at < 0)
            {
                return line;
            }
            string color = level switch
            {
                LogLevel.Trace or LogLevel.Debug => Escape + "37m",
                LogLevel.Information => Escape + "32m",
                LogLevel.Warning => Escape + "33m",
                LogLevel.Error => Escape + "41m" + Escape + "30m",
                LogLevel.Critical => Escape + "41m" + Escape + "37m",
                LogLevel.None or _ => Escape + "39m",
            };
            // Bold, then the color; the reset undoes all three so the message keeps the terminal's own.
            return line[..at] + Escape + "1m" + color + token + Escape + "39m" + Escape + "49m" + Escape + "22m"
                + line[(at + token.Length)..];
        }

        private static bool ShouldColor(LogLevel level)
        {
            return !ColorDisabled
                && (level >= StandardErrorThreshold ? !Console.IsErrorRedirected : !Console.IsOutputRedirected);
        }
    }
}
