using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Util;

public static class BezierUtil
{
    public const int MinPointsForBezier = 3;

    public static int GetBezierLinePointsCount(int originalLineCount, int bezierSegments)
    {
        if (originalLineCount < 3)
        {
            return originalLineCount;
        }
        int num = originalLineCount - 2;
        return num * bezierSegments + 3;
    }

    public static void CreateBezierLine(List<Vector2> source, List<Vector2> target, int bezierSegments)
    {
        if (source.Count < 3)
        {
            target.AddItemsNoGarbage(source);
            return;
        }
        target.Add(source.First());
        for (int i = 0; i < source.Count - 2; i++)
        {
            Vector2 origin = VectorUtil.Center(source[i], source[i + 1]);
            Vector2 destination = VectorUtil.Center(source[i + 1], source[i + 2]);
            Vector2 control = source[i + 1];
            GetBezierPoints(origin, control, destination, bezierSegments, i == source.Count - 3, target);
        }
        target.Add(source.Last());
    }

    public static void AddControlPoints(List<Vector2> target, List<Vector2> points)
    {
        target.Add(points.First());
        target.Add(points.First());
        for (int i = 0; i < points.Count - 1; i++)
        {
            target.Add(VectorUtil.Center(points[i], points[i + 1]));
            target.Add(points[i + 1]);
        }
        target.Add(points.Last());
    }

    public static void AddBezierPoints(List<Vector2> target, List<Vector2> points, int segments)
    {
        for (int i = 0; i < points.Count - 2; i += 2)
        {
            GetBezierPoints(points[i], points[i + 1], points[i + 2], segments, insertLast: false, target);
        }
        target.Add(points.Last());
    }

    public static void AddBezierPoints(List<Vector2> target, List<Vector2> points, List<int> segmentsVector)
    {
        for (int i = 0; i < points.Count - 2; i += 2)
        {
            GetBezierPoints(points[i], points[i + 1], points[i + 2], segmentsVector[i / 2], insertLast: false, target);
        }
        target.Add(points.Last());
    }

    public static void GetBezierPoints(Vector2 origin, Vector2 control, Vector2 destination, int segments, bool insertLast, List<Vector2> result)
    {
        float num = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = (float)Math.Pow(1f - num, 2.0) * origin.X + 2f * (1f - num) * num * control.X + num * num * destination.X;
            float y = (float)Math.Pow(1f - num, 2.0) * origin.Y + 2f * (1f - num) * num * control.Y + num * num * destination.Y;
            result.Add(new Vector2(x, y));
            num += 1f / (float)segments;
        }
        if (insertLast)
        {
            result.Add(destination);
        }
    }
}
