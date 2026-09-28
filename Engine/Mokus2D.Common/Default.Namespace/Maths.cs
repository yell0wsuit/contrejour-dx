using System;

namespace Default.Namespace;

public static class Maths
{
	public const float PI = (float)Math.PI;

	public const float PI3 = (float)Math.PI / 3f;

	public const float PI_X_2 = (float)Math.PI * 2f;

	public const float PI2 = (float)Math.PI / 2f;

	public const float PI4 = (float)Math.PI / 4f;

	private const int DegreesInCircle = 360;

	private static Random randomGenerator = new Random((int)DateTime.Now.Ticks);

	public static Random RandomGenerator => randomGenerator;

	public static void Randomize(int seed)
	{
		randomGenerator = new Random(seed);
	}

	public static float AsymptoticToOne(float ratio)
	{
		return ratio / (1f + ratio);
	}

	public static float AsymptoticTo(float maxValue, float ratio)
	{
		ratio /= maxValue;
		return maxValue * ratio / (1f + ratio);
	}

	public static float InverseLerp(this float value, float from, float to)
	{
		if (to == from)
		{
			return from;
		}
		return (value - from) / (to - from);
	}

	public static float StepTo(float value, float target, float maxStep)
	{
		if (Math.Abs(target - value) <= maxStep)
		{
			return target;
		}
		return value + (float)Math.Sign(target - value) * maxStep;
	}

	public static float Max(float a, float b, float c, float d)
	{
		return Math.Max(Max(a, b, c), d);
	}

	public static int Max(int a, int b, int c, int d)
	{
		return Math.Max(Max(a, b, c), d);
	}

	public static int Max(int a, int b, int c)
	{
		return Math.Max(Math.Max(a, b), c);
	}

	public static float Max(float a, float b, float c)
	{
		return Math.Max(Math.Max(a, b), c);
	}

	public static float Min(float a, float b, float c, float d)
	{
		return Math.Min(Min(a, b, c), d);
	}

	public static float Min(float a, float b, float c)
	{
		return Math.Min(Math.Min(a, b), c);
	}

	public static float Atan2(float y, float x)
	{
		return (float)Math.Atan2(y, x);
	}

	public static float ModPositive(float source, float module)
	{
		float num = source % module;
		if (num < 0f)
		{
			num += module;
		}
		return num;
	}

	public static int ModPositive(int source, int module)
	{
		int num = source % module;
		if (num < 0)
		{
			num += module;
		}
		return num;
	}

	public static float Floor(float source, float module)
	{
		float num = ModPositive(source, module);
		return source - num;
	}

	public static float Ceil(float source, float module)
	{
		float num = ModPositive(source, module);
		return source - num + module;
	}

	public static float Round(float source, float basis, float module)
	{
		float source2 = source - basis;
		return Round(source2, module) + basis;
	}

	public static float Round(float source, float module)
	{
		float num = ModPositive(source, module);
		if (num < module / 2f)
		{
			return source - num;
		}
		return source - num + module;
	}

	public static float PeriodicOffset(float value, float period)
	{
		return value - Round(value, period);
	}

	public static bool Between(float value, float min, float max)
	{
		if (value >= min)
		{
			return value <= max;
		}
		return false;
	}

	public static float Random(float min, float max)
	{
		return Lerp(min, max, Random());
	}

	public static float Lerp(float value1, float value2, float amount)
	{
		return value1 + (value2 - value1) * amount;
	}

	public static float Clamp(float value, float min, float max)
	{
		return Math.Min(Math.Max(value, min), max);
	}

	public static float RandomAngle()
	{
		return Random((float)Math.PI * 2f);
	}

	public static float Random(float max)
	{
		return Random() * max;
	}

	public static int Random(int max)
	{
		return RandomGenerator.Next(max);
	}

	public static float Random()
	{
		return (float)RandomGenerator.NextDouble();
	}

	public static float SimplifyAngle(float angle)
	{
		return angle.SimplifyAngle(0f);
	}

	public static float SimplifyAngle(this float angle, float startValue)
	{
		return SimplifyAngle(angle, startValue, (float)Math.PI * 2f);
	}

	public static float SimplifyAngleDegrees(float value, float startValue)
	{
		return SimplifyAngle(value, startValue, 360f);
	}

	private static float SimplifyAngle(float angle, float startValue, float module)
	{
		angle -= startValue;
		angle = ModPositive(angle, module);
		return angle + startValue;
	}

	public static float Sqrt(float f)
	{
		return (float)Math.Sqrt(f);
	}

	public static float Cos(float f)
	{
		return (float)Math.Cos(f);
	}

	public static float Sin(float f)
	{
		return (float)Math.Sin(f);
	}

	public static float Log(float f)
	{
		return (float)Math.Log(f);
	}

	public static float Clamp(float value, float? min, float? max)
	{
		if (min.HasValue)
		{
			value = Math.Max(min.Value, value);
		}
		if (max.HasValue)
		{
			value = Math.Min(max.Value, value);
		}
		return value;
	}

	public static bool FuzzyEquals(float a, float b, float delta = 0.0001f)
	{
		return Math.Abs(a - b) < delta;
	}

	public static bool FuzzyNotEquals(float a, float b, float delta = 0.0001f)
	{
		return !FuzzyEquals(a, b, delta);
	}

	public static float EaseInOut(float progress, float maxValue)
	{
		return (0f - maxValue) / 2f * ((float)Math.Cos(Math.PI * (double)progress) - 1f);
	}

	public static int Pow2Ceil(int x)
	{
		int num;
		for (num = 1; num < x; num <<= 1)
		{
		}
		return num;
	}

	public static int Pow2Floor(int x)
	{
		int num = 1;
		while (num << 1 <= x)
		{
			num <<= 1;
		}
		return num;
	}

	public static float RandomOffset(float center, float offset)
	{
		return center + Random(0f - offset, offset);
	}
}
