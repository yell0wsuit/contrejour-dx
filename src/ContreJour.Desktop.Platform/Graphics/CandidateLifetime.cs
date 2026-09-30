using System;
using System.Collections.Generic;

namespace ContreJour.Desktop.Platform.Graphics
{
    // Everything one device candidate acquired, released in reverse order. Each acquisition is
    // registered right after it succeeds, so a failure part-way through releases exactly what exists.
    public sealed class CandidateLifetime : IDisposable
    {
        private readonly Stack<IDisposable> _resources = new();

        private bool _disposed;

        public T Own<T>(T resource) where T : IDisposable
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ArgumentNullException.ThrowIfNull(resource);
            _resources.Push(resource);
            return resource;
        }

        // Releases everything even when some releases throw, then reports them together.
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            List<Exception> failures = [];
            while (_resources.TryPop(out IDisposable resource))
            {
                try
                {
                    resource.Dispose();
                }
                catch (Exception failure)
                {
                    failures.Add(failure);
                }
            }
            if (failures.Count != 0)
            {
                throw new AggregateException("Releasing a graphics candidate failed.", failures);
            }
        }
    }
}
