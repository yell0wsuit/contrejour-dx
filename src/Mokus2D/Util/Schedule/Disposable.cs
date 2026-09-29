using System;

using Mokus2D.Util.Resources;

namespace Mokus2D.Util.Schedule;

public class Disposable(Action action) : DisposableBase
{
    private readonly Action action = action;

    protected override void Dispose(bool disposing)
    {
        action?.Invoke();
        base.Dispose(disposing);
    }
}
