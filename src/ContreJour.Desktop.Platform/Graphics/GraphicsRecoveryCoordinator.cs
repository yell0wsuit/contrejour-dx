using System;
using System.Collections.Generic;
using System.Linq;

namespace ContreJour.Desktop.Platform.Graphics
{
    // No renderer could replace a lost device.
    public sealed class GraphicsRecoveryFailedException : Exception
    {
        public GraphicsRecoveryFailedException()
        {
        }

        public GraphicsRecoveryFailedException(string message) : base(message)
        {
        }

        public GraphicsRecoveryFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GraphicsRecoveryFailedException(IReadOnlyList<GraphicsBackendKind> attempted, Exception innerException)
            : base($"No renderer replaced the lost device (tried {string.Join(", ", attempted)}).", innerException)
        {
            Attempted = attempted;
        }

        public IReadOnlyList<GraphicsBackendKind> Attempted { get; } = [];
    }

    // Replaces a lost device, retrying the renderer that was running before the rest of the order:
    // most losses (a driver reset, a GPU switch, a display change) are not the renderer's fault, and
    // the same one comes straight back. A forced renderer is never switched away from.
    public sealed class GraphicsRecoveryCoordinator(string platform, GraphicsBackendKind? forced)
    {
        // A device that keeps dying before it draws anything is not coming back.
        public const int MaximumFramelessRecoveries = 4;

        private int _framelessRecoveries;

        public int Recoveries { get; private set; }

        public GraphicsBackendKind[] OrderAfter(GraphicsBackendKind lost)
        {
            GraphicsBackendKind[] preference = BackendSelector.PreferenceOrder(platform, forced);
            return forced.HasValue ? preference : [lost, .. preference.Where(candidate => candidate != lost)];
        }

        // False once MaximumFramelessRecoveries losses have come in a row with no frame presented.
        public bool TryBeginRecovery()
        {
            _framelessRecoveries++;
            return _framelessRecoveries <= MaximumFramelessRecoveries;
        }

        public void FramePresented()
        {
            _framelessRecoveries = 0;
        }

        // The lost selection is released first, however this ends: its window, context and driver
        // handles are what a replacement needs back. A teardown that throws does not stop the
        // replacement; it is reported only if nothing comes up.
        public GraphicsSelection<T> Recover<T>(GraphicsSelection<T> lost, Func<GraphicsBackendKind, CandidateLifetime, T> create, Action<T> validate)
            where T : IDisposable
        {
            ArgumentNullException.ThrowIfNull(lost);
            GraphicsBackendKind[] order = OrderAfter(lost.Kind);
            Exception teardownFailure = null;
            try
            {
                lost.Dispose();
            }
            catch (Exception failure)
            {
                teardownFailure = failure;
            }
            try
            {
                GraphicsSelection<T> replacement = BackendSelector.Attempt(order, create, validate);
                Recoveries++;
                return replacement;
            }
            catch (AggregateException failures)
            {
                throw new GraphicsRecoveryFailedException(order, teardownFailure == null
                    ? failures
                    : new AggregateException([.. failures.InnerExceptions, teardownFailure]));
            }
        }
    }
}
