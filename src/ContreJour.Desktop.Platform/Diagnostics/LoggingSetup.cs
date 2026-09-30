using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using NReco.Logging.File;

namespace ContreJour.Desktop.Platform.Diagnostics
{
    // The desktop logging pipeline (from cuttherope-dx): one file per run in a logs folder beside the
    // save data, and a console that only speaks up about warnings and worse.
    public static class LoggingSetup
    {
        public const string LevelSwitch = "--log-level";

        private const long FileSizeLimitBytes = 1024 * 1024;

        private const int MaxRollingFiles = 4;

        private const string LogFilePrefix = "contrejour";

        // A name per run means nothing overwrites itself, so this bounds the folder instead: this
        // many files, this run's included.
        private const int MaxRetainedLogFiles = 10;

#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

        // Reads --log-level VALUE or --log-level=VALUE; null when absent. Throws ArgumentException for
        // an unknown level.
        public static LogLevel? ParseLevel(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            string value = null;
            int index = Array.IndexOf(args, LevelSwitch);
            if (index >= 0 && index + 1 < args.Length)
            {
                value = args[index + 1];
            }
            else
            {
                string joined = Array.Find(args, argument => argument.StartsWith(LevelSwitch + "=", StringComparison.Ordinal));
                value = joined?[(LevelSwitch.Length + 1)..];
            }
            return string.IsNullOrEmpty(value)
                ? null
                : value.ToLowerInvariant() switch
                {
                    "trace" => LogLevel.Trace,
                    "debug" => LogLevel.Debug,
                    "info" => LogLevel.Information,
                    "warn" => LogLevel.Warning,
                    "error" => LogLevel.Error,
                    _ => throw new ArgumentException($"Unknown {LevelSwitch} '{value}'. Expected trace, debug, info, warn or error."),
                };
        }

        public static string DirectoryFor(string saveDirectory)
        {
            return Path.Combine(saveDirectory, "logs");
        }

        // The fallback adds the process id, which separates two runs started in the same second.
        public static string LogFileName(DateTime stamp, bool fallback)
        {
            string name = LogFilePrefix + "-" + stamp.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
            return fallback ? $"{name}-{Environment.ProcessId}.log" : name + ".log";
        }

        // The first thing anyone reads on a report, so plain lines rather than log entries.
        public static string ComposeHeader(string product, string version)
        {
            return string.Join(
                Environment.NewLine,
                product,
                Configuration + " build",
                "Version: " + version,
                "OS: " + RuntimeInformation.OSDescription,
                ".NET: " + RuntimeInformation.FrameworkDescription,
                "Architecture: " + RuntimeInformation.ProcessArchitecture);
        }

        public static ILoggerFactory Create(string saveDirectory, LogLevel? requested, string header)
        {
            return Create(saveDirectory, requested, header, DateTime.Now);
        }

        // The caller owns and disposes the factory. Without a requested level the file keeps
        // Information and up and the console Warning and up; a requested level applies to both.
        public static ILoggerFactory Create(string saveDirectory, LogLevel? requested, string header, DateTime stamp)
        {
            LogLevel fileLevel = requested ?? LogLevel.Information;
            LogLevel consoleLevel = requested ?? LogLevel.Warning;
            string directory = DirectoryFor(saveDirectory);
            string path = Path.Combine(directory, LogFileName(stamp, fallback: false));
            bool canWriteFile = TryCreateLogDirectory(directory);
            int fallbackAttempted = 0;
            if (canWriteFile)
            {
                PruneOldLogs(directory);
                WriteHeader(path, header);
            }
            try
            {
                return BuildFactory(canWriteFile);
            }
            catch (IOException)
            {
                return BuildFactory(false);
            }
            catch (UnauthorizedAccessException)
            {
                return BuildFactory(false);
            }

            // NReco suppresses the primary open failure, but opening its fallback can still throw.
            // The file provider is built before the console one so a failed file constructor can
            // fall back without opening a second console writer.
            ILoggerFactory BuildFactory(bool includeFile)
            {
                return LoggerFactory.Create(builder =>
                {
                    _ = builder.SetMinimumLevel(fileLevel < consoleLevel ? fileLevel : consoleLevel);
                    _ = builder.AddFilter<FileLoggerProvider>(null, fileLevel);
                    _ = builder.AddFilter<ConsoleLoggerProvider>(null, consoleLevel);
                    if (includeFile)
                    {
                        _ = builder.AddFile(path, options =>
                        {
                            options.Append = true;
                            options.FileSizeLimitBytes = FileSizeLimitBytes;
                            options.MaxRollingFiles = MaxRollingFiles;
                            options.RollingFilesConvention = FileLoggerOptions.FileRollingConvention.AscendingStableBase;
                            options.FormatLogEntry = FormatEntry;
                            // The provider opens the file as it is built, before the crash handlers
                            // exist: one fallback name, then console only, never a throw.
                            options.HandleFileError = error =>
                            {
                                if (Interlocked.Exchange(ref fallbackAttempted, 1) == 0)
                                {
                                    string fallbackPath = Path.Combine(directory, LogFileName(stamp, fallback: true));
                                    WriteHeader(fallbackPath, header);
                                    error.UseNewLogFileName(fallbackPath);
                                }
                            };
                        });
                    }
                    _ = builder.AddConsole(options =>
                    {
                        options.LogToStandardErrorThreshold = RedactingConsoleFormatter.StandardErrorThreshold;
                        options.FormatterName = RedactingConsoleFormatter.FormatterName;
                    });
                    // Registered directly rather than through AddConsoleFormatter, which binds options
                    // from configuration and is unsafe to trim; this formatter has nothing to bind.
                    _ = builder.Services.AddSingleton<ConsoleFormatter, RedactingConsoleFormatter>();
                });
            }
        }

        public static string FormatEntry(LogMessage entry)
        {
            return LogEntryFormat.Compose(entry.LogLevel, entry.LogName, entry.Message, entry.Exception);
        }

        // A log without its header is still worth having; the provider reports the same failure.
        private static void WriteHeader(string path, string header)
        {
            try
            {
                File.AppendAllText(path, header + Environment.NewLine);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        // Keeps room for the run about to start one; a file another run holds open is left alone.
        private static void PruneOldLogs(string directory)
        {
            try
            {
                FileInfo[] existing = new DirectoryInfo(directory).GetFiles(LogFilePrefix + "-*.log");
                Array.Sort(existing, static (a, b) => b.LastWriteTimeUtc.CompareTo(a.LastWriteTimeUtc));
                for (int i = MaxRetainedLogFiles - 1; i < existing.Length; i++)
                {
                    try
                    {
                        existing[i].Delete();
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static bool TryCreateLogDirectory(string directory)
        {
            try
            {
                _ = Directory.CreateDirectory(directory);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
