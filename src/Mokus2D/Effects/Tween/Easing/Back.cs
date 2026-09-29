using System;

namespace Mokus2D.Effects.Tween.Easing
{
    public class Back
    {
        public static readonly Func<float, float> EaseIn = k => EaseInFunction(k);

        public static readonly Func<float, float> EaseInOut = k => EaseInOutFunction(k);

        public static readonly Func<float, float> EaseOut = k => EaseOutFunction(k);

        public static readonly Func<float, float, float> EaseInWith = EaseInFunction;

        public static readonly Func<float, float, float> EaseInOutWith = EaseInOutFunction;

        public static readonly Func<float, float, float> EaseOutWith = EaseOutFunction;

        private static float EaseInFunction(float k, float s = 1.70158f)
        {
            return k * k * (((s + 1f) * k) - s);
        }

        private static float EaseOutFunction(float k, float s = 1.70158f)
        {
            return ((k -= 1f) * k * (((s + 1f) * k) + s)) + 1f;
        }

        private static float EaseInOutFunction(float k, float s = 1.70158f)
        {
            s *= 1.525f;
            return !((k *= 2f) < 1f) ? 0.5f * (((k -= 2f) * k * (((s + 1f) * k) + s)) + 2f) : 0.5f * (k * k * (((s + 1f) * k) - s));
        }
    }
}
