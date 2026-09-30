using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace ContreJour.Desktop.Platform.Diagnostics
{
    // Writes a failure nothing caught into the log, flushes it, and shows the crash dialog. Without
    // this a crash leaves nothing behind on Windows, where the game has no console.
    public static partial class CrashHandlers
    {
        private static ILoggerFactory _owner;

        private static int _reporting;

        // The factory to flush before the dialog; null forgets it.
        public static void Configure(ILoggerFactory owner)
        {
            _owner = owner;
        }

        // Other threads' failures and unobserved tasks. The main thread's own failure comes through
        // ReportFatal from Program.
        public static void Install()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandled;
            TaskScheduler.UnobservedTaskException += OnUnobserved;
        }

        public static void ReportFatal(Exception failure)
        {
            Report(failure, terminating: true);
        }

        // "The game stopped", the first real failure, and where the log is.
        internal static string Describe(object failure)
        {
            if (failure is AggregateException aggregate)
            {
                AggregateException flattened = aggregate.Flatten();
                if (flattened.InnerExceptions.Count > 0)
                {
                    failure = flattened.InnerExceptions[0];
                }
            }
            string fault = failure is Exception exception
                ? $"{exception.GetType().Name}: {exception.Message}"
                : Convert.ToString(failure, CultureInfo.InvariantCulture) ?? "Unknown failure.";
            string logStatus = CrashDialog.HasFileLog
                ? "A log of this session has been saved."
                : "A log file could not be saved for this session.";
            return $"The game stopped unexpectedly.{Environment.NewLine}{Environment.NewLine}{fault}"
                + $"{Environment.NewLine}{Environment.NewLine}{logStatus}";
        }

        private static void OnUnhandled(object sender, UnhandledExceptionEventArgs args)
        {
            Report(args.ExceptionObject, args.IsTerminating);
        }

        private static void OnUnobserved(object sender, UnobservedTaskExceptionEventArgs args)
        {
            try
            {
                Unobserved(Log.For(LogCategories.Host), args.Exception);
                args.SetObserved();
            }
            catch (Exception)
            {
                // Already failing; a second exception here would bury the first.
            }
        }

        private static void Report(object failure, bool terminating)
        {
            // A second failure while the first is being reported (threads still running behind the
            // dialog) would stack another box, and the log is already closed.
            if (Interlocked.CompareExchange(ref _reporting, 1, 0) != 0)
            {
                return;
            }
            try
            {
                ILogger logger = Log.For(LogCategories.Host);
                if (failure is Exception exception)
                {
                    Unhandled(logger, exception, terminating);
                }
                else if (logger.IsEnabled(LogLevel.Critical))
                {
                    string payload = failure?.ToString() ?? "(none)";
                    UnhandledPayload(logger, terminating, payload);
                }
                // Retired before disposing, so threads still running are not handed loggers from a
                // disposed factory; disposing flushes the file before the dialog offers it.
                Log.Factory = null;
                _owner?.Dispose();
                _owner = null;
                CrashDialog.Show(Describe(failure));
            }
            catch (Exception)
            {
                // Already failing; a second exception here would bury the first.
            }
            finally
            {
                _ = Interlocked.Exchange(ref _reporting, 0);
            }
        }

        [LoggerMessage(Level = LogLevel.Critical, Message = "Unhandled exception, terminating={Terminating}")]
        private static partial void Unhandled(ILogger logger, Exception exception, bool terminating);

        [LoggerMessage(Level = LogLevel.Critical, Message = "Unhandled failure, terminating={Terminating}: {Payload}")]
        private static partial void UnhandledPayload(ILogger logger, bool terminating, string payload);

        [LoggerMessage(Level = LogLevel.Error, Message = "Unobserved task exception")]
        private static partial void Unobserved(ILogger logger, Exception exception);
    }
}
