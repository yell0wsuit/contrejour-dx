namespace ContreJour.Debug;

public static class DebugUtil
{
    public static void Trace(string value, params object[] args)
    {
        System.Diagnostics.Trace.TraceInformation(value, args);
    }

    public static void Trace(string value)
    {
        System.Diagnostics.Trace.TraceInformation(value);
    }
}
