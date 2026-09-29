using System;
using System.Collections.Generic;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening;

namespace Mokus2D.Effects.Tween.ValueSetters;

public static class Setters
{
    internal static Dictionary<Type, Pool<ValueSetter>> Pools;

    static Setters()
    {
        Pools = [];
        InitializePool(() => new FloatSetter());
        InitializePool(() => new ColorSetter());
        InitializePool(() => new IntSetter());
        InitializePool(() => new Vector2Setter());
        InitializePool(() => new Vector3Setter());
        InitializePool(() => new BoolSetter());
    }

    private static void InitializePool<T>(Func<ValueSetter<T>> activator)
    {
        Pool<ValueSetter> value = new(activator);
        Pools.Add(typeof(T), value);
    }

    public static ValueSetter<TValue> New<TValue>(object target, GetSetValue<TValue> getSet, TValue targetValue, EasingData easing)
    {
        Pool<ValueSetter> pool = Pools[typeof(TValue)];
        ValueSetter<TValue> valueSetter = (ValueSetter<TValue>)pool.New();
        return valueSetter.Initialize(target, getSet, targetValue, easing);
    }

    public static void Free(ValueSetter property)
    {
        Pool<ValueSetter> pool = Pools[property.ValueType];
        pool.Free(property);
    }
}
