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
            return Filter != null ? Filter() : true;
        }
    }

    public TouchListenerDecorator(ITouchListener listener, bool enabled = true)
        : base(enabled)
    {
        _listener = listener;
    }

    public override bool TouchBegin(Touch touch)
    {
        return FilterValue ? _listener.TouchBegin(touch) : false;
    }

    public override bool TouchMove(Touch touch)
    {
        return FilterValue ? _listener.TouchMove(touch) : false;
    }

    public override void TouchEnd(Touch touch)
    {
        if (FilterValue)
        {
            _listener.TouchEnd(touch);
        }
    }
}
