using System;

public class SimpleProfiler : IDisposable
{
    public SimpleProfiler(string aName, int aLevel = 0)
    {
    }

    static SimpleProfiler()
    {
    }

    public void Stop()
    {
    }

    public void Dispose()
    {
    }

    public static SimpleProfiler Track(string aName, int aLevel = 0)
    {
        return new SimpleProfiler(aName, aLevel);
    }
}
