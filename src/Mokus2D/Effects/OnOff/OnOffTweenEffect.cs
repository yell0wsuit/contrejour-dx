using System;

using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class OnOffTweenEffect<TValue>(Node target, float duration, GetSetValue<Node, TValue> valueSetter, TValue onValue, TValue offValue) : OnOffTimeEffect(target, duration)
{
    private readonly GetSetValue<Node, TValue> _valueSetter = valueSetter;

    public bool Clean { get; set; }

    public int? Tag { get; set; }

    public Func<float, float> Easing { get; set; }

    public Action OnComplete { get; set; }

    public TValue OnValue { get; private set; } = onValue;

    public TValue OffValue { get; private set; } = offValue;

    public virtual void ResetOnValue(TValue value)
    {
        OnValue = value;
    }

    public virtual void ResetOffValue(TValue value)
    {
        OffValue = value;
    }

    protected override void SetOn()
    {
        _ = TweenTo(OnValue, on: true);
    }

    protected override void SetOff()
    {
        _ = TweenTo(OffValue, on: false);
    }

    protected TweenObject TweenTo(TValue targetValue, bool on)
    {
        TryStopTween();
        TweenObject tweenObject = Target.Tweener.Start(GetDuration(on), Tag).Tween(_valueSetter, targetValue, Easing);
        if (OnComplete != null)
        {
            _ = tweenObject.OnComplete(OnComplete);
        }
        return tweenObject;
    }

    private void TryStopTween()
    {
        if (Clean)
        {
            if (Tag.HasValue)
            {
                Target.Tweener.Stop(Tag.Value);
            }
            else
            {
                Target.Tweener.Stop();
            }
        }
    }

    public override void SetOn(bool value)
    {
        TryStopTween();
        base.SetOn(value);
        _valueSetter.SetValue(Target, value ? OnValue : OffValue);
    }
}
