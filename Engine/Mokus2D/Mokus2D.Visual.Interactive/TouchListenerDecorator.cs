using System;

using Mokus2D.Input;

namespace Mokus2D.Visual.Interactive;

public class TouchListenerDecorator(ITouchListener listener, bool enabled = true) : TouchListenerBase(enabled)
{
    private readonly ITouchListener _listener = listener;

    public Func<bool> Filter;

    private bool FilterValue => Filter == null || Filter();

    public override bool TouchBegin(Touch touch)
    {
        return FilterValue && _listener.TouchBegin(touch);
    }

    public override bool TouchMove(Touch touch)
    {
        return FilterValue && _listener.TouchMove(touch);
    }

    public override void TouchEnd(Touch touch)
    {
        if (FilterValue)
        {
            _listener.TouchEnd(touch);
        }
    }
}
