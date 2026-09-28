using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public static class ContreDrawUtil
{
    public static Pair<Vector2> ccp2Pair(Pair<Vector2> pair)
    {
        return new Pair<Vector2>(pair.First, pair.Second);
    }

    public static Pair<Vector2> GetPointsPair(Vector2 center, Vector2 start, Vector2 end, float width)
    {
        return VectorUtil.GetOrthoPoints(center, start, end, width);
    }

    public static Pair<Vector2> GetPointsPairStartEndWidthResult(Vector2 center, Vector2 start, Vector2 end, float width)
    {
        Vector2 center2 = Box2DConfig.DefaultConfig.ToPoint(center);
        Vector2 start2 = Box2DConfig.DefaultConfig.ToPoint(start);
        Vector2 end2 = Box2DConfig.DefaultConfig.ToPoint(end);
        width /= 1f / 30f;
        return GetPointsPair(center2, start2, end2, width);
    }

    public static List<Vector2> CreateBezierLineBezierMaxBezierStep(List<Vector2> line, float maxStep)
    {
        List<Vector2> list = new List<Vector2>();
        list.Add(line[0]);
        for (int i = 1; i < line.Count - 1; i++)
        {
            Vector2 vector = line[i - 1].Middle(line[i]);
            Vector2 vector2 = line[i].Middle(line[i + 1]);
            int num = (int)Math.Ceiling((vector.DistanceTo(line[i]) + vector2.DistanceTo(line[i])) / maxStep);
            if (num <= 1)
            {
                list.Add(vector);
            }
            else
            {
                BezierUtil.GetBezierPoints(insertLast: i == line.Count - 1, origin: vector, control: line[i], destination: vector2, segments: num, result: list);
            }
        }
        list.Add(line.Last());
        return list;
    }

    public static void CreateBezierSurfaceSurfaceSegments(List<Vector2> polygon, ref List<Vector2> surface, int segments)
    {
        Vector2 vector = (polygon[0] + polygon[1]) * 0.5f;
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 origin = vector;
            Vector2 vector2 = polygon[(i + 1) % polygon.Count];
            vector = (vector2 + polygon[(i + 2) % polygon.Count]) * 0.5f;
            BezierUtil.GetBezierPoints(origin, vector2, vector, segments, insertLast: false, surface);
        }
    }

    public static void CreateBezierPointsSurfaceSegments(List<Vector2> polygon, ref List<Vector2> surface, int segments)
    {
        for (int i = 0; i < polygon.Count - 1; i += 2)
        {
            BezierUtil.GetBezierPoints(destination: polygon[(i != polygon.Count - 2) ? (i + 2) : 0], origin: polygon[i], control: polygon[i + 1], segments: segments, insertLast: false, result: surface);
        }
    }
}
