using System.Numerics;

namespace FarseerPhysics.Common
{
    // MonoGame 3.8.5.1's Vector2.CatmullRom and MathHelper.CatmullRom (MIT License, Copyright (C) The
    // MonoGame Team, https://github.com/MonoGame/MonoGame); System.Numerics has no spline helpers.
    internal static class CatmullRom
    {
        public static Vector2 Interpolate(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
        {
            return new Vector2(Interpolate(value1.X, value2.X, value3.X, value4.X, amount), Interpolate(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        private static float Interpolate(float value1, float value2, float value3, float value4, float amount)
        {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using doubles not to lose precision
            double amountSquared = amount * amount;
            double amountCubed = amountSquared * amount;
            return (float)(0.5 * ((2.0 * value2)
                + ((value3 - value1) * amount)
                + (((2.0 * value1) - (5.0 * value2) + (4.0 * value3) - value4) * amountSquared)
                + (((3.0 * value2) - value1 - (3.0 * value3) + value4) * amountCubed)));
        }
    }
}
