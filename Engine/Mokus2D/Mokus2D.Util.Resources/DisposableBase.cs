using System;

namespace Mokus2D.Util.Resources;

public class DisposableBase : IDisposable
{
    private bool _isDisposed;

    public bool IsDisposed => _isDisposed;

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    ~DisposableBase()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            Dispose(disposing: false);
        }
    }
}
