using System;

namespace Mokus2D.Effects.Tween.Easing
{
    public class Sine
    {
        public static readonly Func<float, float> EaseIn = EaseInFunction;

        public static readonly Func<float, float> EaseInOut = EaseInOutFunction;

        public static readonly Func<float, float> EaseOut = EaseOutFunction;

        private static float EaseInFunction(float k)
        {
            return (float)(1.0 - Math.Cos(k * ((float)Math.PI / 2f)));
        }

        private static float EaseInOutFunction(float k)
        {
            return (float)((0.0 - (Math.Cos((float)Math.PI * k) - 1.0)) / 2.0);
        }

        private static float EaseOutFunction(float k)
        {
            return (float)Math.Sin(k * ((float)Math.PI / 2f));
        }
    }
}
