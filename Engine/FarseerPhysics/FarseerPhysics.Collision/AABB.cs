using System;

using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public struct AABB
{
    public Vector2 LowerBound;

    public Vector2 UpperBound;

    public readonly float Width => UpperBound.X - LowerBound.X;

    public readonly float Height => UpperBound.Y - LowerBound.Y;

    public readonly Vector2 Center => 0.5f * (LowerBound + UpperBound);

    public readonly Vector2 Extents => 0.5f * (UpperBound - LowerBound);

    public readonly float Perimeter
    {
        get
        {
            float num = UpperBound.X - LowerBound.X;
            float num2 = UpperBound.Y - LowerBound.Y;
            return 2f * (num + num2);
        }
    }

    public readonly Vertices Vertices
    {
        get
        {
            Vertices vertices =
            [
                UpperBound,
                new Vector2(UpperBound.X, LowerBound.Y),
                LowerBound,
                new Vector2(LowerBound.X, UpperBound.Y),
            ];
            return vertices;
        }
    }

    public readonly AABB Q1 => new(Center, UpperBound);

    public readonly AABB Q2 => new(new Vector2(LowerBound.X, Center.Y), new Vector2(Center.X, UpperBound.Y));

    public readonly AABB Q3 => new(LowerBound, Center);

    public readonly AABB Q4 => new(new Vector2(Center.X, LowerBound.Y), new Vector2(UpperBound.X, Center.Y));

    public AABB(Vector2 min, Vector2 max)
    {
        this = new AABB(ref min, ref max);
    }

    public AABB(ref Vector2 min, ref Vector2 max)
    {
        LowerBound = min;
        UpperBound = max;
    }

    public AABB(Vector2 center, float width, float height)
    {
        LowerBound = center - new Vector2(width / 2f, height / 2f);
        UpperBound = center + new Vector2(width / 2f, height / 2f);
    }

    public readonly bool IsValid()
    {
        Vector2 vector = UpperBound - LowerBound;
        return vector.X >= 0f && vector.Y >= 0f && LowerBound.IsValid() && UpperBound.IsValid();
    }

    public void Combine(ref AABB aabb)
    {
        LowerBound = Vector2.Min(LowerBound, aabb.LowerBound);
        UpperBound = Vector2.Max(UpperBound, aabb.UpperBound);
    }

    public void Combine(ref AABB aabb1, ref AABB aabb2)
    {
        LowerBound = Vector2.Min(aabb1.LowerBound, aabb2.LowerBound);
        UpperBound = Vector2.Max(aabb1.UpperBound, aabb2.UpperBound);
    }

    public readonly bool Contains(ref AABB aabb)
    {
        return true && LowerBound.X <= aabb.LowerBound.X && LowerBound.Y <= aabb.LowerBound.Y && aabb.UpperBound.X <= UpperBound.X && aabb.UpperBound.Y <= UpperBound.Y;
    }

    public readonly bool Contains(ref Vector2 point)
    {
        return point.X > LowerBound.X + 1.1920929E-07f && point.X < UpperBound.X - 1.1920929E-07f
            ? point.Y > LowerBound.Y + 1.1920929E-07f && point.Y < UpperBound.Y - 1.1920929E-07f
            : false;
    }

    public static bool TestOverlap(ref AABB a, ref AABB b)
    {
        Vector2 vector = b.LowerBound - a.UpperBound;
        Vector2 vector2 = a.LowerBound - b.UpperBound;
        return vector.X > 0f || vector.Y > 0f ? false : vector2.X <= 0f && vector2.Y <= 0f;
    }

    public readonly bool RayCast(out RayCastOutput output, ref RayCastInput input, bool doInteriorCheck = true)
    {
        output = default;
        float num = float.MinValue;
        float num2 = float.MaxValue;
        Vector2 point = input.Point1;
        Vector2 v = input.Point2 - input.Point1;
        Vector2 vector = MathUtils.Abs(v);
        Vector2 zero = Vector2.Zero;
        for (int i = 0; i < 2; i++)
        {
            float num3 = (i == 0) ? vector.X : vector.Y;
            float num4 = (i == 0) ? LowerBound.X : LowerBound.Y;
            float num5 = (i == 0) ? UpperBound.X : UpperBound.Y;
            float num6 = (i == 0) ? point.X : point.Y;
            if (num3 < 1.1920929E-07f)
            {
                if (num6 < num4 || num5 < num6)
                {
                    return false;
                }
                continue;
            }
            float num7 = (i == 0) ? v.X : v.Y;
            float num8 = 1f / num7;
            float a = (num4 - num6) * num8;
            float b = (num5 - num6) * num8;
            float num9 = -1f;
            if (a > b)
            {
                MathUtils.Swap(ref a, ref b);
                num9 = 1f;
            }
            if (a > num)
            {
                if (i == 0)
                {
                    zero.X = num9;
                }
                else
                {
                    zero.Y = num9;
                }
                num = a;
            }
            num2 = Math.Min(num2, b);
            if (num > num2)
            {
                return false;
            }
        }
        if (doInteriorCheck && (num < 0f || input.MaxFraction < num))
        {
            return false;
        }
        output.Fraction = num;
        output.Normal = zero;
        return true;
    }
}
