using System;

namespace Mokus2D.Visual.GameDebug;

public struct TimeCalculator : IDisposable
{
    private DateTime _start;

    public readonly float ElapsedSeconds => (float)(DateTime.UtcNow - _start).TotalSeconds;

    public static TimeCalculator Create()
    {
        TimeCalculator result = default;
        result.Start();
        return result;
    }

    public void Start()
    {
        _start = DateTime.UtcNow;
    }

    public readonly void Dispose()
    {
    }
}
