using System;

namespace Mokus2D.Effects.Tween.Easing;

public class Expo
{
    public static readonly Func<float, float> EaseIn = EaseInFunction;

    public static readonly Func<float, float> EaseInOut = EaseInOutFunction;

    public static readonly Func<float, float> EaseOut = EaseOutFunction;

    private static float EaseInFunction(float k)
    {
        return (float)((k == 0f) ? 0.0 : Math.Pow(2.0, 10f * (k - 1f)));
    }

    private static float EaseInOutFunction(float k)
    {
        return k == 0f
            ? 0f
            : k == 1f
            ? 1f
            : (float)(((k *= 2f) < 1f) ? (0.5 * Math.Pow(2.0, 10f * (k - 1f))) : (0.5 * (0.0 - Math.Pow(2.0, -10f * (k - 1f)) + 2.0)));
    }

    private static float EaseOutFunction(float k)
    {
        return (float)((k == 1f) ? 1.0 : (0.0 - Math.Pow(2.0, -10f * k) + 1.0));
    }
}
