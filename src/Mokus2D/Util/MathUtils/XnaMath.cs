using System;
using System.Numerics;

namespace Mokus2D.Util.MathUtils
{
    // MonoGame's formulas for the math whose System.Numerics version gives different bits (or treats
    // ±0 and NaN differently), so moving to System.Numerics keeps the regression output byte-identical.
    // Temporary: each method names the System.Numerics call that replaces it afterwards.
    // Bodies are MonoGame 3.8.5.1's (MIT License, Copyright (C) The MonoGame Team,
    // https://github.com/MonoGame/MonoGame).
    public static class XnaMath
    {
        // Becomes `value / divider`.
        public static Vector2 Divide(Vector2 value, float divider)
        {
            float factor = 1 / divider;
            value.X *= factor;
            value.Y *= factor;
            return value;
        }

        // Becomes `value / divider`.
        public static Vector3 Divide(Vector3 value, float divider)
        {
            float factor = 1 / divider;
            value.X *= factor;
            value.Y *= factor;
            value.Z *= factor;
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

        // Becomes `float.Lerp(value1, value2, amount)`.
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + ((value2 - value1) * amount);
        }

        // Becomes `Vector2.Lerp(value1, value2, amount)`.
        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            return new Vector2(Lerp(value1.X, value2.X, amount), Lerp(value1.Y, value2.Y, amount));
        }

        // Becomes `Vector3.Lerp(value1, value2, amount)`.
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            return new Vector3(Lerp(value1.X, value2.X, amount), Lerp(value1.Y, value2.Y, amount), Lerp(value1.Z, value2.Z, amount));
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

        // Becomes `Vector2.Transform(position, matrix)`.
        public static Vector2 Transform(Vector2 position, in Matrix4x4 matrix)
        {
            return new Vector2((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41, (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
        }

        // Becomes `matrix1 * matrix2`.
        public static Matrix4x4 Multiply(in Matrix4x4 matrix1, in Matrix4x4 matrix2)
        {
            float m11 = (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21) + (matrix1.M13 * matrix2.M31) + (matrix1.M14 * matrix2.M41);
            float m12 = (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22) + (matrix1.M13 * matrix2.M32) + (matrix1.M14 * matrix2.M42);
            float m13 = (matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23) + (matrix1.M13 * matrix2.M33) + (matrix1.M14 * matrix2.M43);
            float m14 = (matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24) + (matrix1.M13 * matrix2.M34) + (matrix1.M14 * matrix2.M44);
            float m21 = (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21) + (matrix1.M23 * matrix2.M31) + (matrix1.M24 * matrix2.M41);
            float m22 = (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22) + (matrix1.M23 * matrix2.M32) + (matrix1.M24 * matrix2.M42);
            float m23 = (matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23) + (matrix1.M23 * matrix2.M33) + (matrix1.M24 * matrix2.M43);
            float m24 = (matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24) + (matrix1.M23 * matrix2.M34) + (matrix1.M24 * matrix2.M44);
            float m31 = (matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21) + (matrix1.M33 * matrix2.M31) + (matrix1.M34 * matrix2.M41);
            float m32 = (matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22) + (matrix1.M33 * matrix2.M32) + (matrix1.M34 * matrix2.M42);
            float m33 = (matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23) + (matrix1.M33 * matrix2.M33) + (matrix1.M34 * matrix2.M43);
            float m34 = (matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24) + (matrix1.M33 * matrix2.M34) + (matrix1.M34 * matrix2.M44);
            float m41 = (matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21) + (matrix1.M43 * matrix2.M31) + (matrix1.M44 * matrix2.M41);
            float m42 = (matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22) + (matrix1.M43 * matrix2.M32) + (matrix1.M44 * matrix2.M42);
            float m43 = (matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23) + (matrix1.M43 * matrix2.M33) + (matrix1.M44 * matrix2.M43);
            float m44 = (matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24) + (matrix1.M43 * matrix2.M34) + (matrix1.M44 * matrix2.M44);
            return new Matrix4x4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
        }

        // Becomes `Matrix.Invert(matrix, out Matrix result)` (its bool result discarded).
        public static Matrix4x4 Invert(in Matrix4x4 matrix)
        {
            float num1 = matrix.M11;
            float num2 = matrix.M12;
            float num3 = matrix.M13;
            float num4 = matrix.M14;
            float num5 = matrix.M21;
            float num6 = matrix.M22;
            float num7 = matrix.M23;
            float num8 = matrix.M24;
            float num9 = matrix.M31;
            float num10 = matrix.M32;
            float num11 = matrix.M33;
            float num12 = matrix.M34;
            float num13 = matrix.M41;
            float num14 = matrix.M42;
            float num15 = matrix.M43;
            float num16 = matrix.M44;
            float num17 = (float)(((double)num11 * num16) - ((double)num12 * num15));
            float num18 = (float)(((double)num10 * num16) - ((double)num12 * num14));
            float num19 = (float)(((double)num10 * num15) - ((double)num11 * num14));
            float num20 = (float)(((double)num9 * num16) - ((double)num12 * num13));
            float num21 = (float)(((double)num9 * num15) - ((double)num11 * num13));
            float num22 = (float)(((double)num9 * num14) - ((double)num10 * num13));
            float num23 = (float)(((double)num6 * num17) - ((double)num7 * num18) + ((double)num8 * num19));
            float num24 = (float)-(((double)num5 * num17) - ((double)num7 * num20) + ((double)num8 * num21));
            float num25 = (float)(((double)num5 * num18) - ((double)num6 * num20) + ((double)num8 * num22));
            float num26 = (float)-(((double)num5 * num19) - ((double)num6 * num21) + ((double)num7 * num22));
            float num27 = (float)(1.0 / (((double)num1 * num23) + ((double)num2 * num24) + ((double)num3 * num25) + ((double)num4 * num26)));
            float m11 = num23 * num27;
            float m21 = num24 * num27;
            float m31 = num25 * num27;
            float m41 = num26 * num27;
            float m12 = (float)-(((double)num2 * num17) - ((double)num3 * num18) + ((double)num4 * num19)) * num27;
            float m22 = (float)(((double)num1 * num17) - ((double)num3 * num20) + ((double)num4 * num21)) * num27;
            float m32 = (float)-(((double)num1 * num18) - ((double)num2 * num20) + ((double)num4 * num22)) * num27;
            float m42 = (float)(((double)num1 * num19) - ((double)num2 * num21) + ((double)num3 * num22)) * num27;
            float num28 = (float)(((double)num7 * num16) - ((double)num8 * num15));
            float num29 = (float)(((double)num6 * num16) - ((double)num8 * num14));
            float num30 = (float)(((double)num6 * num15) - ((double)num7 * num14));
            float num31 = (float)(((double)num5 * num16) - ((double)num8 * num13));
            float num32 = (float)(((double)num5 * num15) - ((double)num7 * num13));
            float num33 = (float)(((double)num5 * num14) - ((double)num6 * num13));
            float m13 = (float)(((double)num2 * num28) - ((double)num3 * num29) + ((double)num4 * num30)) * num27;
            float m23 = (float)-(((double)num1 * num28) - ((double)num3 * num31) + ((double)num4 * num32)) * num27;
            float m33 = (float)(((double)num1 * num29) - ((double)num2 * num31) + ((double)num4 * num33)) * num27;
            float m43 = (float)-(((double)num1 * num30) - ((double)num2 * num32) + ((double)num3 * num33)) * num27;
            float num34 = (float)(((double)num7 * num12) - ((double)num8 * num11));
            float num35 = (float)(((double)num6 * num12) - ((double)num8 * num10));
            float num36 = (float)(((double)num6 * num11) - ((double)num7 * num10));
            float num37 = (float)(((double)num5 * num12) - ((double)num8 * num9));
            float num38 = (float)(((double)num5 * num11) - ((double)num7 * num9));
            float num39 = (float)(((double)num5 * num10) - ((double)num6 * num9));
            float m14 = (float)-(((double)num2 * num34) - ((double)num3 * num35) + ((double)num4 * num36)) * num27;
            float m24 = (float)(((double)num1 * num34) - ((double)num3 * num37) + ((double)num4 * num38)) * num27;
            float m34 = (float)-(((double)num1 * num35) - ((double)num2 * num37) + ((double)num4 * num39)) * num27;
            float m44 = (float)(((double)num1 * num36) - ((double)num2 * num38) + ((double)num3 * num39)) * num27;
            return new Matrix4x4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
        }

        // Becomes `Matrix.CreateRotationX(radians)`.
        public static Matrix4x4 CreateRotationX(float radians)
        {
            Matrix4x4 result = Matrix4x4.Identity;
            float val1 = MathF.Cos(radians);
            float val2 = MathF.Sin(radians);
            result.M22 = val1;
            result.M23 = val2;
            result.M32 = -val2;
            result.M33 = val1;
            return result;
        }

        // Becomes `Matrix.CreateRotationZ(radians)`.
        public static Matrix4x4 CreateRotationZ(float radians)
        {
            Matrix4x4 result = Matrix4x4.Identity;
            float val1 = MathF.Cos(radians);
            float val2 = MathF.Sin(radians);
            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;
            return result;
        }

        // Becomes `float.DegreesToRadians(degrees)`.
        public static float ToRadians(float degrees)
        {
            return (float)(degrees * 0.017453292519943295769236907684886);
        }

        // Becomes `float.RadiansToDegrees(radians)`.
        public static float ToDegrees(float radians)
        {
            return (float)(radians * 57.295779513082320876798154814105);
        }
    }
}
