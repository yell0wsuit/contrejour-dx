using System;

using Mokus2D.Data;
using Mokus2D.Interfaces;

namespace Mokus2D.Effects.Tweening;

public interface ITween : ICleanable, IUpdatable
{
    bool Finished { get; }

    void Free();

    void Reset();
}
public interface ITween<out T> : ICompletableTween, ITween, ICleanable, IUpdatable where T : ITween<T>
{
    T Tween<TValue>(GetSetValue<TValue> getSet, TValue targetValue, Func<float, float, float, float> easing, float easingParamA, float easingParamB);

    T Tween<TValue>(GetSetValue<TValue> getSet, TValue targetValue, Func<float, float, float> easing, float easingParam);

    T Tween<TValue>(GetSetValue<TValue> getSet, TValue targetValue, Func<float, float> easing);

    T Tween<TValue>(GetSetValue<TValue> getSet, TValue targetValue);

    T OnComplete(Action<object> action);

    new T OnComplete(Action action);
}
