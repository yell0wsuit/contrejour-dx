using System;

namespace ContreJourDX.Browser.Platform
{
    // The game's variable timestep in the browser: seconds between animation frames. A frame run by a wake is
    // stamped with performance.now(), and the next animation frame carries its begin time, which can be earlier;
    // such a step is zero rather than negative, and time is counted from the later stamp so none is owed twice.
    // Reset makes the next step zero, as the desktop host restarts its clock on activation.
    public sealed class FrameClock
    {
        private double _previousMs = double.NaN;

        public void Reset()
        {
            _previousMs = double.NaN;
        }

        public float Advance(double timestampMs)
        {
            double previous = _previousMs;
            if (double.IsNaN(previous))
            {
                _previousMs = timestampMs;
                return 0f;
            }
            _previousMs = Math.Max(previous, timestampMs);
            return (float)(Math.Max(0, timestampMs - previous) / 1000.0);
        }
    }
}
