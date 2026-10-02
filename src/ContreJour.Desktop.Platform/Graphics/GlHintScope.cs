using System;
using System.Collections.Generic;

using SDL3;

namespace ContreJour.Desktop.Platform.Graphics
{
    // Sets SDL hints for one renderer attempt and puts back what was there before. SDL's hints are
    // process-wide, so a value left behind would belong to whichever renderer is tried next. A hint
    // SDL refuses (it does, at normal priority, when the matching environment variable is set) fails
    // the attempt, which falls through to the next candidate and honors what the environment asked.
    public sealed class GlHintScope : IDisposable
    {
        private readonly Func<string, string> _read;

        private readonly Func<string, string, bool> _write;

        private readonly Func<string, bool> _clear;

        private readonly List<(string Name, string Previous)> _displaced = [];

        private bool _disposed;

        public GlHintScope() : this(SDL.GetHint, SDL.SetHint, SDL.ResetHint)
        {
        }

        internal GlHintScope(Func<string, string> read, Func<string, string, bool> write, Func<string, bool> clear)
        {
            _read = read;
            _write = write;
            _clear = clear;
        }

        public void Set(string name, string value)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ArgumentException.ThrowIfNullOrEmpty(name);
            string previous = _read(name);
            if (!_write(name, value))
            {
                throw new InvalidOperationException($"SDL would not set the hint '{name}': {SDL.GetError()}");
            }
            // Recorded only once the write took, so a refused hint is not "restored".
            _displaced.Add((name, previous));
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            for (int index = _displaced.Count - 1; index >= 0; index--)
            {
                (string name, string previous) = _displaced[index];
                _ = previous == null ? _clear(name) : _write(name, previous);
            }
            _displaced.Clear();
        }
    }
}
