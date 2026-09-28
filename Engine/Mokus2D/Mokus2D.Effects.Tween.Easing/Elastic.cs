using System;

namespace Mokus2D.Effects.Tween.Easing;

public class Elastic
{
    public static readonly Func<float, float> EaseIn = k => EaseInFunction(k);

    public static readonly Func<float, float> EaseOut = k => EaseOutFunction(k);

    public static Func<float, float> EaseInOut = k => EaseInOutFunction(k);

    public static readonly Func<float, float, float> EaseInWith = (k, a) => EaseInFunction(k, a);

    public static readonly Func<float, float, float> EaseInOutWith = (k, a) => EaseInOutFunction(k, a);

    public static readonly Func<float, float, float> EaseOutWith = (k, a) => EaseOutFunction(k, a);

    public static readonly Func<float, float, float, float> EaseInWith2 = EaseInFunction;

    public static readonly Func<float, float, float, float> EaseInOutWith2 = EaseInOutFunction;

    public static readonly Func<float, float, float, float> EaseOutWith2 = EaseOutFunction;

    private static float EaseInFunction(float k, float a = 0.1f, float p = 0.4f)
    {
        if (k == 0f)
        {
            return 0f;
        }
        if (k == 1f)
        {
            return 1f;
        }
        if (p == 0f)
        {
            p = 0.3f;
        }
        float num;
        if (a is 0f or < 1f)
        {
            a = 1f;
            num = p / 4f;
        }
        else
        {
            num = (float)((double)(p / ((float)Math.PI * 2f)) * Math.Asin(1f / a));
        }
        return (float)(0.0 - ((double)a * Math.Pow(2.0, 10f * (k -= 1f)) * Math.Sin((k - num) * ((float)Math.PI * 2f) / p)));
    }

    private static float EaseOutFunction(float k, float a = 0.1f, float p = 0.4f)
    {
        if (k == 0f)
        {
            return 0f;
        }
        if (k == 1f)
        {
            return 1f;
        }
        if (p == 0f)
        {
            p = 0.3f;
        }
        float num;
        if (a is 0f or < 1f)
        {
            a = 1f;
            num = p / 4f;
        }
        else
        {
            num = (float)((double)(p / ((float)Math.PI * 2f)) * Math.Asin(1f / a));
        }
        return (float)(((double)a * Math.Pow(2.0, -10f * k) * Math.Sin((k - num) * ((float)Math.PI * 2f) / p)) + 1.0);
    }

    private static float EaseInOutFunction(float k, float a = 0.1f, float p = 0.4f)
    {
        if (k == 0f)
        {
            return 0f;
        }
        if (k == 1f)
        {
            return 1f;
        }
        if (p == 0f)
        {
            p = 0.3f;
        }
        float num;
        if (a is 0f or < 1f)
        {
            a = 1f;
            num = p / 4f;
        }
        else
        {
            num = (float)((double)(p / ((float)Math.PI * 2f)) * Math.Asin(1f / a));
        }
        return (float)(((k *= 2f) < 1f) ? (-0.5 * ((double)a * Math.Pow(2.0, 10f * (k -= 1f)) * Math.Sin((k - num) * ((float)Math.PI * 2f) / p))) : (((double)a * Math.Pow(2.0, -10f * (k -= 1f)) * Math.Sin((k - num) * ((float)Math.PI * 2f) / p) * 0.5) + 1.0));
    }
}
