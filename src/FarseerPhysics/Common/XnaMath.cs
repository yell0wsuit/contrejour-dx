using System;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common
{
    // MonoGame's formulas for the math whose System.Numerics version gives different bits (or treats
    // ±0 and NaN differently), so moving to System.Numerics keeps the regression output byte-identical.
    // Temporary: each method names the System.Numerics call that replaces it afterwards.
    // Bodies are MonoGame 3.8.5.1's (MIT License, Copyright (C) The MonoGame Team,
    // https://github.com/MonoGame/MonoGame).
    internal static class XnaMath
    {
        // Becomes `value / divider`.
        public static Vector2 Divide(Vector2 value, float divider)
        {
            float factor = 1 / divider;
            value.X *= factor;
            value.Y *= factor;
            return value;
        }

        // Becomes `Vector2.Normalize(value)`.
        public static Vector2 Normalize(Vector2 value)
        {
            float val = 1.0f / MathF.Sqrt((value.X * value.X) + (value.Y * value.Y));
            value.X *= val;
            value.Y *= val;
            return value;
        }

        // Becomes `Vector2.Min(value1, value2)`.
        public static Vector2 Min(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X < value2.X ? value1.X : value2.X, value1.Y < value2.Y ? value1.Y : value2.Y);
        }

        // Becomes `Vector2.Max(value1, value2)`.
        public static Vector2 Max(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X > value2.X ? value1.X : value2.X, value1.Y > value2.Y ? value1.Y : value2.Y);
        }

        // Becomes `MathF.Min(value1, value2)`.
        public static float Min(float value1, float value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        // Becomes `Vector2.Transform(position, matrix)`.
        public static Vector2 Transform(Vector2 position, in Matrix matrix)
        {
            return new Vector2((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41, (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
        }

        // Becomes `Matrix.CreateRotationZ(radians)`.
        public static Matrix CreateRotationZ(float radians)
        {
            Matrix result = Matrix.Identity;
            float val1 = MathF.Cos(radians);
            float val2 = MathF.Sin(radians);
            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;
            return result;
        }
    }
}
