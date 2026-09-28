using System;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening;

namespace Mokus2D.Effects.Tween.ValueSetters;

public abstract class ValueSetter
{
    public abstract Type ValueType { get; }

    public abstract void SetValue(float ratio);

    public abstract void Start();

    public abstract void SetTarget();
}
public abstract class ValueSetter<TValue> : ValueSetter, ICleanable
{
    private GetSetValue<TValue> _getSet;

    private TValue _targetValue;

    private TValue _startValue;

    private object _target;

    private EasingData _easing;

    public override Type ValueType => typeof(TValue);

    public override void SetTarget()
    {
        _getSet.SetValue(_target, _targetValue);
    }

    public override void SetValue(float ratio)
    {
        if (_easing != null)
        {
            ratio = _easing.Ease(ratio);
        }
        TValue value = Lerp(_startValue, _targetValue, ratio);
        _getSet.SetValue(_target, value);
    }

    public override void Start()
    {
        _startValue = _getSet.GetValue(_target);
    }

    public ValueSetter<TValue> Initialize(object target, GetSetValue<TValue> getSet, TValue targetValue, EasingData easing = null)
    {
        _getSet = getSet;
        _targetValue = targetValue;
        _target = target;
        _easing = easing;
        return this;
    }

    protected abstract TValue Lerp(TValue from, TValue to, float amount);

    public void Clean()
    {
        _target = null;
        if (_easing != null)
        {
            EasingData.Free(_easing);
            _easing = null;
        }
    }
}
