using System;


using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Util.MathUtils;

public static class VectorUtil
{
    public static Pair<Vector2> GetOrthoPoints(Vector2 center, Vector2 start, Vector2 end, float width)
    {
        return GetOrthoPoints(center, end - start, width);
    }

    public static Pair<Vector2> GetOrthoPoints(Vector2 center, Vector2 direction, float width)
    {
        Vector2 value = direction.Rotate90();
        _ = Normalize(ref value, width / 2f);
        return new Pair<Vector2>(center + value, center - value);
    }

    public static float Atan2(Vector2 source)
    {
        return (float)Math.Atan2(source.Y, source.X);
    }

    public static float Atan2(Vector2 source, Vector2 target)
    {
        return Atan2(target - source);
    }

    public static Vector2 Random(Vector2 start, Vector2 end)
    {
        return new Vector2(Maths.Random(start.X, end.X), Maths.Random(start.Y, end.Y));
    }

    public static Vector2 Random(Vector2 value)
    {
        return new Vector2(Maths.Random(value.X), Maths.Random(value.Y));
    }

    public static Vector2 ClampDistance(this Vector2 vec, Vector2 center, float maxDistance)
    {
        Vector2 vector = vec - center;
        float num = vector.Length();
        if (num < maxDistance)
        {
            return vec;
        }
        vector *= maxDistance / num;
        return vector + center;
    }

    public static Vector2 Rotate(Vector2 point, Vector2 center, float angle)
    {
        Vector2 point2 = point - center;
        return center + Rotate(point2, angle);
    }

    public static Vector2 Rotate(Vector2 point, float angle)
    {
        float num = Maths.Cos(angle);
        float num2 = Maths.Sin(angle);
        return new Vector2((point.X * num) - (point.Y * num2), (point.Y * num) + (point.X * num2));
    }

    public static void RotateMinus90(ref Vector2 point)
    {
        float x = point.X;
        point.X = point.Y;
        point.Y = 0f - x;
    }

    public static void Rotate90(ref Vector2 point)
    {
        float x = point.X;
        point.X = 0f - point.Y;
        point.Y = x;
    }

    public static Vector2 StepTo(Vector2 source, Vector2 target, float maxStep)
    {
        Vector2 source2 = target - source;
        if (source2.Length() < maxStep)
        {
            return target;
        }
        float angle = Atan2(source2);
        source += ToVector(maxStep, angle);
        return source;
    }

    public static Vector2 Normalize(ref Vector2 value, float length)
    {
        value.Normalize();
        value *= length;
        return value;
    }

    public static Vector2 PointInDirection(Vector2 center, Vector2 direction, float module)
    {
        Vector2 source = direction - center;
        source = source.Normalize(module);
        return source + center;
    }

    public static Vector2 ClampLength(ref Vector2 vec, float maxLength)
    {
        float num = vec.Length();
        if (num > maxLength)
        {
            vec *= maxLength / num;
        }
        return vec;
    }

    public static Vector2 EnsureLength(this Vector2 vec, float minLength)
    {
        return vec.LengthSquared() < minLength * minLength ? vec.Normalize(minLength) : vec;
    }

    public static Vector2 ClampLength(this Vector2 vec, float maxLength)
    {
        _ = ClampLength(ref vec, maxLength);
        return vec;
    }

    public static Vector2 ToVector(float module, float angle)
    {
        return new Vector2((float)((double)module * Math.Cos(angle)), (float)((double)module * Math.Sin(angle)));
    }

    public static Vector2 Center(Vector2 first, Vector2 second)
    {
        return (first + second) / 2f;
    }

    public static bool FuzzyEquals(this Vector2 a, Vector2 b, float delta = 0.0001f)
    {
        return Maths.FuzzyEquals(a.X, b.X, delta) && Maths.FuzzyEquals(a.Y, b.Y, delta);
    }

    public static Vector2 Clamp(this Vector2 position, Vector2 minValue, Vector2 maxValue)
    {
        return new Vector2(position.X.Clamp(minValue.X, maxValue.X), position.Y.Clamp(minValue.Y, maxValue.Y));
    }

    public static float WherePoint(Vector2 start, Vector2 end, Vector2 point)
    {
        Vector2 vector = end - start;
        Vector2 vector2 = point - start;
        return (vector.X * vector2.Y) - (vector.Y * vector2.X);
    }

    public static Vector2 VectorProjection(Vector2 source, Vector2 target)
    {
        float num = Vector2.Dot(source, target);
        float num2 = target.LengthSquared();
        if (Maths.FuzzyEquals(num2, 0f))
        {
            return new Vector2(0f, 0f);
        }
        num /= num2;
        return new Vector2(num * target.X, num * target.Y);
    }

    public static float Projection(Vector2 source, Vector2 target)
    {
        return Vector2.Dot(source, target) / target.Length();
    }

    public static float DistanceToSegment(this Vector2 vector, Vector2 segmentStart, Vector2 segmentEnd)
    {
        return vector.DistanceTo(vector.GetClosestPoint(segmentStart, segmentEnd));
    }

    public static Vector2 GetClosestPoint(this Vector2 vector, Vector2 segmentStart, Vector2 segmentEnd)
    {
        Vector2 vector2 = segmentEnd - segmentStart;
        Vector2 value = vector - segmentStart;
        double num = Vector2.Dot(value, vector2);
        if (num <= 0.0)
        {
            return segmentStart;
        }
        double num2 = Vector2.Dot(vector2, vector2);
        if (num2 <= num)
        {
            return segmentEnd;
        }
        float num3 = (float)(num / num2);
        return segmentStart + (num3 * vector2);
    }
}
