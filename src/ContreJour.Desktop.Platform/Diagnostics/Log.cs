using System;
using System.Collections.Concurrent;
using System.Threading;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ContreJour.Desktop.Platform.Diagnostics
{
    // The logging seam (from cuttherope-dx). The desktop host installs a factory at startup; until
    // it does, and in tests that install none, every logger is a no-op.
    public static class Log
    {
        // Each lookup uses one snapshot of the factory and its logger cache; replacing the factory
        // publishes a fresh cache.
        private sealed record State(ILoggerFactory Factory, ConcurrentDictionary<string, ILogger> Loggers);

        private static State _state = NewState(NullLoggerFactory.Instance);

        // Assigning null restores the no-op factory.
        public static ILoggerFactory Factory
        {
            get => Volatile.Read(ref _state).Factory;
            set => Volatile.Write(ref _state, NewState(value ?? NullLoggerFactory.Instance));
        }

        public static ILogger For(string category)
        {
            State current = Volatile.Read(ref _state);
            return current.Loggers.GetOrAdd(category, static (name, factory) => factory.CreateLogger(name), current.Factory);
        }

        private static State NewState(ILoggerFactory factory)
        {
            return new State(factory, new ConcurrentDictionary<string, ILogger>(StringComparer.Ordinal));
        }
    }

    public static class LogCategories
    {
        public const string Graphics = "ContreJour.Graphics";

        public const string Host = "ContreJour.Host";

        public const string Audio = "ContreJour.Audio";
    }
}
