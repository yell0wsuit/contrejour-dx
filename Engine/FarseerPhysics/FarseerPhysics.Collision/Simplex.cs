using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

internal struct Simplex
{
	internal int Count;

	internal FixedArray3<SimplexVertex> V;

	internal void ReadCache(ref SimplexCache cache, DistanceProxy proxyA, ref Transform transformA, DistanceProxy proxyB, ref Transform transformB)
	{
		Count = cache.Count;
		for (int i = 0; i < Count; i++)
		{
			SimplexVertex value = V[i];
			value.IndexA = cache.IndexA[i];
			value.IndexB = cache.IndexB[i];
			Vector2 v = proxyA.Vertices[value.IndexA];
			Vector2 v2 = proxyB.Vertices[value.IndexB];
			value.WA = MathUtils.Mul(ref transformA, v);
			value.WB = MathUtils.Mul(ref transformB, v2);
			value.W = value.WB - value.WA;
			value.A = 0f;
			V[i] = value;
		}
		if (Count > 1)
		{
			float metric = cache.Metric;
			float metric2 = GetMetric();
			if (metric2 < 0.5f * metric || 2f * metric < metric2 || metric2 < 1.1920929E-07f)
			{
				Count = 0;
			}
		}
		if (Count == 0)
		{
			SimplexVertex value2 = V[0];
			value2.IndexA = 0;
			value2.IndexB = 0;
			Vector2 v3 = proxyA.Vertices[0];
			Vector2 v4 = proxyB.Vertices[0];
			value2.WA = MathUtils.Mul(ref transformA, v3);
			value2.WB = MathUtils.Mul(ref transformB, v4);
			value2.W = value2.WB - value2.WA;
			value2.A = 1f;
			V[0] = value2;
			Count = 1;
		}
	}

	internal void WriteCache(ref SimplexCache cache)
	{
		cache.Metric = GetMetric();
		cache.Count = (ushort)Count;
		for (int i = 0; i < Count; i++)
		{
			cache.IndexA[i] = (byte)V[i].IndexA;
			cache.IndexB[i] = (byte)V[i].IndexB;
		}
	}

	internal Vector2 GetSearchDirection()
	{
		switch (Count)
		{
		case 1:
			return -V[0].W;
		case 2:
		{
			Vector2 a = V[1].W - V[0].W;
			float num = MathUtils.Cross(a, -V[0].W);
			if (num > 0f)
			{
				return new Vector2(0f - a.Y, a.X);
			}
			return new Vector2(a.Y, 0f - a.X);
		}
		default:
			return Vector2.Zero;
		}
	}

	internal Vector2 GetClosestPoint()
	{
		return Count switch
		{
			0 => Vector2.Zero, 
			1 => V[0].W, 
			2 => V[0].A * V[0].W + V[1].A * V[1].W, 
			3 => Vector2.Zero, 
			_ => Vector2.Zero, 
		};
	}

	internal void GetWitnessPoints(out Vector2 pA, out Vector2 pB)
	{
		switch (Count)
		{
		case 0:
			pA = Vector2.Zero;
			pB = Vector2.Zero;
			break;
		case 1:
			pA = V[0].WA;
			pB = V[0].WB;
			break;
		case 2:
			pA = V[0].A * V[0].WA + V[1].A * V[1].WA;
			pB = V[0].A * V[0].WB + V[1].A * V[1].WB;
			break;
		case 3:
			pA = V[0].A * V[0].WA + V[1].A * V[1].WA + V[2].A * V[2].WA;
			pB = pA;
			break;
		default:
			throw new Exception();
		}
	}

	internal float GetMetric()
	{
		return Count switch
		{
			0 => 0f, 
			1 => 0f, 
			2 => (V[0].W - V[1].W).Length(), 
			3 => MathUtils.Cross(V[1].W - V[0].W, V[2].W - V[0].W), 
			_ => 0f, 
		};
	}

	internal void Solve2()
	{
		Vector2 w = V[0].W;
		Vector2 w2 = V[1].W;
		Vector2 value = w2 - w;
		float num = 0f - Vector2.Dot(w, value);
		if (num <= 0f)
		{
			SimplexVertex value2 = V[0];
			value2.A = 1f;
			V[0] = value2;
			Count = 1;
			return;
		}
		float num2 = Vector2.Dot(w2, value);
		if (num2 <= 0f)
		{
			SimplexVertex value3 = V[1];
			value3.A = 1f;
			V[1] = value3;
			Count = 1;
			V[0] = V[1];
		}
		else
		{
			float num3 = 1f / (num2 + num);
			SimplexVertex value4 = V[0];
			SimplexVertex value5 = V[1];
			value4.A = num2 * num3;
			value5.A = num * num3;
			V[0] = value4;
			V[1] = value5;
			Count = 2;
		}
	}

	internal void Solve3()
	{
		Vector2 w = V[0].W;
		Vector2 w2 = V[1].W;
		Vector2 w3 = V[2].W;
		Vector2 vector = w2 - w;
		float num = Vector2.Dot(w, vector);
		float num2 = Vector2.Dot(w2, vector);
		float num3 = num2;
		float num4 = 0f - num;
		Vector2 vector2 = w3 - w;
		float num5 = Vector2.Dot(w, vector2);
		float num6 = Vector2.Dot(w3, vector2);
		float num7 = num6;
		float num8 = 0f - num5;
		Vector2 value = w3 - w2;
		float num9 = Vector2.Dot(w2, value);
		float num10 = Vector2.Dot(w3, value);
		float num11 = num10;
		float num12 = 0f - num9;
		float num13 = MathUtils.Cross(vector, vector2);
		float num14 = num13 * MathUtils.Cross(w2, w3);
		float num15 = num13 * MathUtils.Cross(w3, w);
		float num16 = num13 * MathUtils.Cross(w, w2);
		if (num4 <= 0f && num8 <= 0f)
		{
			SimplexVertex value2 = V[0];
			value2.A = 1f;
			V[0] = value2;
			Count = 1;
		}
		else if (num3 > 0f && num4 > 0f && num16 <= 0f)
		{
			float num17 = 1f / (num3 + num4);
			SimplexVertex value3 = V[0];
			SimplexVertex value4 = V[1];
			value3.A = num3 * num17;
			value4.A = num4 * num17;
			V[0] = value3;
			V[1] = value4;
			Count = 2;
		}
		else if (num7 > 0f && num8 > 0f && num15 <= 0f)
		{
			float num18 = 1f / (num7 + num8);
			SimplexVertex value5 = V[0];
			SimplexVertex value6 = V[2];
			value5.A = num7 * num18;
			value6.A = num8 * num18;
			V[0] = value5;
			V[2] = value6;
			Count = 2;
			V[1] = V[2];
		}
		else if (num3 <= 0f && num12 <= 0f)
		{
			SimplexVertex value7 = V[1];
			value7.A = 1f;
			V[1] = value7;
			Count = 1;
			V[0] = V[1];
		}
		else if (num7 <= 0f && num11 <= 0f)
		{
			SimplexVertex value8 = V[2];
			value8.A = 1f;
			V[2] = value8;
			Count = 1;
			V[0] = V[2];
		}
		else if (num11 > 0f && num12 > 0f && num14 <= 0f)
		{
			float num19 = 1f / (num11 + num12);
			SimplexVertex value9 = V[1];
			SimplexVertex value10 = V[2];
			value9.A = num11 * num19;
			value10.A = num12 * num19;
			V[1] = value9;
			V[2] = value10;
			Count = 2;
			V[0] = V[2];
		}
		else
		{
			float num20 = 1f / (num14 + num15 + num16);
			SimplexVertex value11 = V[0];
			SimplexVertex value12 = V[1];
			SimplexVertex value13 = V[2];
			value11.A = num14 * num20;
			value12.A = num15 * num20;
			value13.A = num16 * num20;
			V[0] = value11;
			V[1] = value12;
			V[2] = value13;
			Count = 3;
		}
	}
}
