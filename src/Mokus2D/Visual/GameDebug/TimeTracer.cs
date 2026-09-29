using System;
using System.Diagnostics;

namespace Mokus2D.Visual.GameDebug
{
    public sealed class TimeTracer : IDisposable
    {
        private readonly DateTime _start = DateTime.UtcNow;

        public void Dispose()
        {
            Trace.TraceInformation("elapsed time {0} milliseconds", (DateTime.UtcNow - _start).TotalMilliseconds);
        }
    }
}
