using System;

using Default.Namespace;

namespace Mokus2D.Util.MathUtils;

public static class MathExtensions
{
    private static readonly byte[] bytes = new byte[4];

    private static readonly float[] floats = new float[1];

    public static int ToIntBytes(this float value)
    {
        floats[0] = value;
        Buffer.BlockCopy(floats, 0, bytes, 0, bytes.Length);
        int num = 0;
        for (int i = 0; i < bytes.Length; i++)
        {
            num = (num << 8) + bytes[i];
        }
        return num;
    }

    public static float Fraction(this float value)
    {
        return value - (int)value;
    }

    public static float Abs(this float value)
    {
        return Math.Abs(value);
    }

    public static int Abs(this int value)
    {
        return Math.Abs(value);
    }

    public static int Ceiling(this float value)
    {
        return (int)Math.Ceiling(value);
    }

    public static int Floor(this float value)
    {
        return (int)Math.Floor(value);
    }

    public static float Sign(this float value)
    {
        return Math.Sign(value);
    }

    public static float Sign(this int value)
    {
        return Math.Sign(value);
    }

    public static float Round(this float value)
    {
        return (float)Math.Round(value);
    }

    public static bool Between(this int value, float min, float max)
    {
        return Maths.Between(value, min, max);
    }

    public static bool Between(this float value, float min, float max)
    {
        return Maths.Between(value, min, max);
    }

    public static int Clamp(this int value, int min, int max)
    {
        return Math.Min(Math.Max(value, min), max);
    }

    public static float Clamp(this float value, float min, float max)
    {
        return Maths.Clamp(value, min, max);
    }

    public static float Min(this float value, float min)
    {
        return Math.Min(value, min);
    }

    public static float Max(this float value, float max)
    {
        return Math.Max(value, max);
    }

    public static int Min(this int value, int min)
    {
        return Math.Min(value, min);
    }

    public static int Max(this int value, int max)
    {
        return Math.Max(value, max);
    }

    public static float Clamp(this double value, float min, float max)
    {
        return Maths.Clamp((float)value, min, max);
    }

    public static float StepTo(this float value, float target, float step)
    {
        return Maths.StepTo(value, target, step);
    }

    public static float Lerp(this float amount, float from, float to)
    {
        return Maths.Lerp(from, to, amount);
    }

    public static T RandomItem<T>(this T[] array)
    {
        return array[Maths.RandomGenerator.Next(array.Length)];
    }

    public static float Range(this Random random, float min, float max)
    {
        return Maths.Lerp(min, max, (float)random.NextDouble());
    }

    public static bool FuzzyEquals(this float a, float b, float delta = 0.0001f)
    {
        return Maths.FuzzyEquals(a, b, delta);
    }
}
