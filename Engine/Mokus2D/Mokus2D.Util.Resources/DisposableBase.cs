using System;

namespace Mokus2D.Util.Resources;

public class DisposableBase : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    ~DisposableBase()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            Dispose(disposing: false);
        }
    }
}
