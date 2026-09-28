using System;

using Mokus2D.Effects.Tweening;
using Mokus2D.Util;
using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class FadeAndHideEffect : OnOffTweenEffect<float>
{
    public bool ResetZeroOpacity = true;

    public GetSetValue<Node, bool> VisibleSetter = NodeValues.Visible;

    private readonly Action _onHide;

    public event Action<FadeAndHideEffect> VisibleChangeEvent;

    public FadeAndHideEffect(Node target, float duration)
        : this(target, duration, 1f)
    {
    }

    public FadeAndHideEffect(Node target, float duration, float onOpacity)
        : base(target, duration, NodeValues.OpacityFloat, onOpacity, 0f)
    {
        _onHide = OnHide;
    }

    protected override void SetOn()
    {
        VisibleSetter.SetValue(Target, value: true);
        VisibleChangeEvent.Dispatch(this);
        if (ResetZeroOpacity)
        {
            Target.OpacityFloat = 0f;
        }
        base.SetOn();
    }

    protected override void SetOff()
    {
        _ = TweenTo(OffValue, on: false).SetAfter(VisibleSetter, targetValue: false).OnComplete(_onHide);
    }

    private void OnHide()
    {
        VisibleChangeEvent.Dispatch(this);
    }

    public override void SetOn(bool value)
    {
        base.SetOn(value);
        VisibleSetter.SetValue(Target, value);
        VisibleChangeEvent.Dispatch(this);
    }
}
