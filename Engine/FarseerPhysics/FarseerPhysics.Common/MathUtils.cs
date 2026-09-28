using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public static class MathUtils
{
	[StructLayout(LayoutKind.Explicit)]
	private struct FloatConverter
	{
		[FieldOffset(0)]
		public float x;

		[FieldOffset(0)]
		public int i;
	}

	public static float Cross(ref Vector2 a, ref Vector2 b)
	{
		return a.X * b.Y - a.Y * b.X;
	}

	public static float Cross(Vector2 a, Vector2 b)
	{
		return Cross(ref a, ref b);
	}

	public static Vector3 Cross(Vector3 a, Vector3 b)
	{
		return new Vector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
	}

	public static Vector2 Cross(Vector2 a, float s)
	{
		return new Vector2(s * a.Y, (0f - s) * a.X);
	}

	public static Vector2 Cross(float s, Vector2 a)
	{
		return new Vector2((0f - s) * a.Y, s * a.X);
	}

	public static Vector2 Abs(Vector2 v)
	{
		return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
	}

	public static Vector2 Mul(ref Mat22 A, Vector2 v)
	{
		return Mul(ref A, ref v);
	}

	public static Vector2 Mul(ref Mat22 A, ref Vector2 v)
	{
		return new Vector2(A.ex.X * v.X + A.ey.X * v.Y, A.ex.Y * v.X + A.ey.Y * v.Y);
	}

	public static Vector2 Mul(ref Transform T, Vector2 v)
	{
		return Mul(ref T, ref v);
	}

	public static Vector2 Mul(ref Transform T, ref Vector2 v)
	{
		float x = T.q.c * v.X - T.q.s * v.Y + T.p.X;
		float y = T.q.s * v.X + T.q.c * v.Y + T.p.Y;
		return new Vector2(x, y);
	}

	public static Vector2 MulT(ref Mat22 A, Vector2 v)
	{
		return MulT(ref A, ref v);
	}

	public static Vector2 MulT(ref Mat22 A, ref Vector2 v)
	{
		return new Vector2(v.X * A.ex.X + v.Y * A.ex.Y, v.X * A.ey.X + v.Y * A.ey.Y);
	}

	public static Vector2 MulT(ref Transform T, Vector2 v)
	{
		return MulT(ref T, ref v);
	}

	public static Vector2 MulT(ref Transform T, ref Vector2 v)
	{
		float num = v.X - T.p.X;
		float num2 = v.Y - T.p.Y;
		float x = T.q.c * num + T.q.s * num2;
		float y = (0f - T.q.s) * num + T.q.c * num2;
		return new Vector2(x, y);
	}

	public static void MulT(ref Mat22 A, ref Mat22 B, out Mat22 C)
	{
		C = default(Mat22);
		C.ex.X = A.ex.X * B.ex.X + A.ex.Y * B.ex.Y;
		C.ex.Y = A.ey.X * B.ex.X + A.ey.Y * B.ex.Y;
		C.ey.X = A.ex.X * B.ey.X + A.ex.Y * B.ey.Y;
		C.ey.Y = A.ey.X * B.ey.X + A.ey.Y * B.ey.Y;
	}

	public static Vector3 Mul(Mat33 A, Vector3 v)
	{
		return v.X * A.ex + v.Y * A.ey + v.Z * A.ez;
	}

	public static Transform Mul(Transform A, Transform B)
	{
		return new Transform
		{
			q = Mul(A.q, B.q),
			p = Mul(A.q, B.p) + A.p
		};
	}

	public static void MulT(ref Transform A, ref Transform B, out Transform C)
	{
		C = default(Transform);
		C.q = MulT(A.q, B.q);
		C.p = MulT(A.q, B.p - A.p);
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T val = a;
		a = b;
		b = val;
	}

	public static Vector2 Mul22(Mat33 A, Vector2 v)
	{
		return new Vector2(A.ex.X * v.X + A.ey.X * v.Y, A.ex.Y * v.X + A.ey.Y * v.Y);
	}

	public static Rot Mul(Rot q, Rot r)
	{
		Rot result = default(Rot);
		result.s = q.s * r.c + q.c * r.s;
		result.c = q.c * r.c - q.s * r.s;
		return result;
	}

	public static Vector2 MulT(Transform T, Vector2 v)
	{
		float num = v.X - T.p.X;
		float num2 = v.Y - T.p.Y;
		float x = T.q.c * num + T.q.s * num2;
		float y = (0f - T.q.s) * num + T.q.c * num2;
		return new Vector2(x, y);
	}

	public static Rot MulT(Rot q, Rot r)
	{
		Rot result = default(Rot);
		result.s = q.c * r.s - q.s * r.c;
		result.c = q.c * r.c + q.s * r.s;
		return result;
	}

	public static Transform MulT(Transform A, Transform B)
	{
		return new Transform
		{
			q = MulT(A.q, B.q),
			p = MulT(A.q, B.p - A.p)
		};
	}

	public static Vector2 Mul(Rot q, Vector2 v)
	{
		return new Vector2(q.c * v.X - q.s * v.Y, q.s * v.X + q.c * v.Y);
	}

	public static Vector2 MulT(Rot q, Vector2 v)
	{
		return new Vector2(q.c * v.X + q.s * v.Y, (0f - q.s) * v.X + q.c * v.Y);
	}

	public static Vector2 Skew(Vector2 input)
	{
		return new Vector2(0f - input.Y, input.X);
	}

	public static bool IsValid(float x)
	{
		if (float.IsNaN(x))
		{
			return false;
		}
		return !float.IsInfinity(x);
	}

	public static bool IsValid(this Vector2 x)
	{
		if (IsValid(x.X))
		{
			return IsValid(x.Y);
		}
		return false;
	}

	public static float InvSqrt(float x)
	{
		FloatConverter floatConverter = default(FloatConverter);
		floatConverter.x = x;
		float num = 0.5f * x;
		floatConverter.i = 1597463007 - (floatConverter.i >> 1);
		x = floatConverter.x;
		x *= 1.5f - num * x * x;
		return x;
	}

	public static int Clamp(int a, int low, int high)
	{
		return Math.Max(low, Math.Min(a, high));
	}

	public static float Clamp(float a, float low, float high)
	{
		return Math.Max(low, Math.Min(a, high));
	}

	public static Vector2 Clamp(Vector2 a, Vector2 low, Vector2 high)
	{
		return Vector2.Max(low, Vector2.Min(a, high));
	}

	public static void Cross(ref Vector2 a, ref Vector2 b, out float c)
	{
		c = a.X * b.Y - a.Y * b.X;
	}

	public static double VectorAngle(ref Vector2 p1, ref Vector2 p2)
	{
		double num = Math.Atan2(p1.Y, p1.X);
		double num2 = Math.Atan2(p2.Y, p2.X);
		double num3;
		for (num3 = num2 - num; num3 > Math.PI; num3 -= Math.PI * 2.0)
		{
		}
		for (; num3 < -Math.PI; num3 += Math.PI * 2.0)
		{
		}
		return num3;
	}

	public static float Dot(Vector3 a, Vector3 b)
	{
		return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
	}

	public static double VectorAngle(Vector2 p1, Vector2 p2)
	{
		return VectorAngle(ref p1, ref p2);
	}

	public static float Area(Vector2 a, Vector2 b, Vector2 c)
	{
		return Area(ref a, ref b, ref c);
	}

	public static float Area(ref Vector2 a, ref Vector2 b, ref Vector2 c)
	{
		return a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y);
	}

	public static bool IsCollinear(ref Vector2 a, ref Vector2 b, ref Vector2 c, float tolerance = 0f)
	{
		return FloatInRange(Area(ref a, ref b, ref c), 0f - tolerance, tolerance);
	}

	public static void Cross(float s, ref Vector2 a, out Vector2 b)
	{
		b = new Vector2((0f - s) * a.Y, s * a.X);
	}

	public static bool FloatEquals(float value1, float value2)
	{
		return Math.Abs(value1 - value2) <= 1.1920929E-07f;
	}

	public static bool FloatEquals(float value1, float value2, float delta)
	{
		return FloatInRange(value1, value2 - delta, value2 + delta);
	}

	public static bool FloatInRange(float value, float min, float max)
	{
		if (value >= min)
		{
			return value <= max;
		}
		return false;
	}

	public static Vector2 Mul(ref Rot rot, Vector2 axis)
	{
		return Mul(rot, axis);
	}

	public static Vector2 MulT(ref Rot rot, Vector2 axis)
	{
		return MulT(rot, axis);
	}
}
