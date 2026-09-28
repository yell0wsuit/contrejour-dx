using System;

using Mokus2D.Input;

namespace Mokus2D.Visual.Interactive;

public class TouchListenerDecorator : TouchListenerBase
{
    private readonly ITouchListener _listener;

    public Func<bool> Filter;

    private bool FilterValue
    {
        get
        {
            if (Filter != null)
            {
                return Filter();
            }
            return true;
        }
    }

    public TouchListenerDecorator(ITouchListener listener, bool enabled = true)
        : base(enabled)
    {
        _listener = listener;
    }

    public override bool TouchBegin(Touch touch)
    {
        if (FilterValue)
        {
            return _listener.TouchBegin(touch);
        }
        return false;
    }

    public override bool TouchMove(Touch touch)
    {
        if (FilterValue)
        {
            return _listener.TouchMove(touch);
        }
        return false;
    }

    public override void TouchEnd(Touch touch)
    {
        if (FilterValue)
        {
            _listener.TouchEnd(touch);
        }
    }
}
