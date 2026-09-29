using System;

namespace Mokus2D.Effects.Tween.Easing;

public class Quint
{
    public static readonly Func<float, float> EaseIn = EaseInFunction;

    public static readonly Func<float, float> EaseInOut = EaseInOutFunction;

    public static readonly Func<float, float> EaseOut = EaseOutFunction;

    private static float EaseInFunction(float k)
    {
        return k * k * k * k * k;
    }

    private static float EaseInOutFunction(float k)
    {
        return !((k *= 2f) < 1f) ? 0.5f * (((k -= 2f) * k * k * k * k) + 2f) : 0.5f * k * k * k * k * k;
    }

    private static float EaseOutFunction(float k)
    {
        return ((k -= 1f) * k * k * k * k) + 1f;
    }
}
