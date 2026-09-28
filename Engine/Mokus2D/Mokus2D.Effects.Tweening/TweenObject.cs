using System;
using System.Collections.Generic;

using Mokus2D.Data;
using Mokus2D.Effects.Tween.ValueSetters;
using Mokus2D.Interfaces;

namespace Mokus2D.Effects.Tweening;

public class TweenObject : IntervalTweenBase, ITween<TweenObject>, ICompletableTween, ITween, ICleanable, IUpdatable
{
    private static readonly Pool<TweenObject> Pool = new(() => new TweenObject());

    private object _target;

    private EasingData _easing;

    private readonly List<ValueSetter> _properties = [];

    private readonly List<ValueSetter> _onStart = [];

    private readonly List<ValueSetter> _onEnd = [];

    private readonly Queue<Action<object>> _onCompleteWith = new(64);

    private readonly Queue<TargetAndAction> _onCompleteWithTarget = new(64);

    public static TweenObject New(object target, float seconds)
    {
        return Pool.New().Initialize(target, seconds);
    }

    public static void Free(TweenObject tween)
    {
        Pool.Free(tween);
    }

    private TweenObject()
    {
    }

    public TweenObject OnComplete(object target, Action<object> action)
    {
        if (action == null)
        {
            throw new NullReferenceException("action can not be null");
        }
        _onCompleteWithTarget.Enqueue(new TargetAndAction(target, action));
        return this;
    }

    public TweenObject OnComplete(Action<object> action)
    {
        if (action == null)
        {
            throw new NullReferenceException("action can not be null");
        }
        _onCompleteWith.Enqueue(action);
        return this;
    }

    public TweenObject OnComplete(Action action)
    {
        return (TweenObject)((ICompletableTween)this).OnComplete(action);
    }

    private TweenObject AddSetter<TValue>(GetSetValue<TValue> getSet, TValue targetValue, EasingData easing, List<ValueSetter> list)
    {
        ValueSetter<TValue> item = Setters.New(_target, getSet, targetValue, easing);
        list.Add(item);
        return this;
    }

    protected override void UpdateRatio(float ratio)
    {
        if (_easing != null)
        {
            ratio = _easing.Ease(ratio);
        }
        foreach (ValueSetter property in _properties)
        {
            property.SetValue(ratio);
        }
    }

    protected override void Start()
    {
        foreach (ValueSetter property in _properties)
        {
            property.Start();
        }
        SetTargetValues(_onStart);
    }

    protected override void Finish()
    {
        SetTargetValues(_onEnd);
        base.Finish();
        while (!_onCompleteWith.Empty())
        {
            _onCompleteWith.Dequeue()(_target);
        }
        while (!_onCompleteWithTarget.Empty())
        {
            _onCompleteWithTarget.Dequeue().Execute();
        }
    }

    private void SetTargetValues(List<ValueSetter> list)
    {
        foreach (ValueSetter item in list)
        {
            item.SetTarget();
        }
    }

    private TweenObject Initialize(object target, float seconds)
    {
        Reset();
        Initialize(seconds);
        _target = target;
        return this;
    }

    public override void Clean()
    {
        base.Clean();
        _onCompleteWith.Clear();
        _onCompleteWithTarget.Clear();
        _onStart.Clear();
        _onEnd.Clear();
        foreach (ValueSetter property in _properties)
        {
            Setters.Free(property);
        }
        _properties.Clear();
        _target = null;
        if (_easing != null)
        {
            EasingData.Free(_easing);
            _easing = null;
        }
    }

    public override void Free()
    {
        Free(this);
    }

    public TweenObject Tween<TValue>(GetSetValue<TValue> getSet, TValue targetValue, Func<float, float, float, float> easing, float easingParamA, float easingParamB)
    {
        return AddSetter(getSet, targetValue, EasingData.NewOrNull(easing, easingParamA, easingParamB), _properties);
    }

    public TweenObject Tween<T>(GetSetValue<T> getSet, T targetValue, Func<float, float, float> easing, float easingParam)
    {
        return AddSetter(getSet, targetValue, EasingData.NewOrNull(easing, easingParam), _properties);
    }

    public TweenObject Tween<T>(GetSetValue<T> getSet, T targetValue, Func<float, float> easing)
    {
        return AddSetter(getSet, targetValue, EasingData.NewOrNull(easing), _properties);
    }

    public TweenObject Tween<T>(GetSetValue<T> getSet, T targetValue)
    {
        return AddSetter(getSet, targetValue, null, _properties);
    }

    public TweenObject Set<T>(GetSetValue<T> getSet, T targetValue)
    {
        return AddSetter(getSet, targetValue, null, _onStart);
    }

    public TweenObject SetAfter<T>(GetSetValue<T> getSet, T targetValue)
    {
        return AddSetter(getSet, targetValue, null, _onEnd);
    }

    public TweenObject Ease(Func<float, float> easing)
    {
        _easing = EasingData.NewOrNull(easing);
        return this;
    }

    public TweenObject Ease(Func<float, float, float> easing, float easingParam)
    {
        _easing = EasingData.NewOrNull(easing, easingParam);
        return this;
    }

    public TweenObject Ease(Func<float, float, float, float> easing, float easingParamA, float easingParamB)
    {
        _easing = EasingData.NewOrNull(easing, easingParamA, easingParamB);
        return this;
    }
}
