using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PolygonManipulation;

public static class SimplifyTools
{
    public static Vertices CollinearSimplify(Vertices vertices, float collinearityTolerance = 0f)
    {
        if (vertices.Count <= 3)
        {
            return vertices;
        }
        Vertices vertices2 = new(vertices.Count);
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 a = vertices.PreviousVertex(i);
            Vector2 b = vertices[i];
            Vector2 c = vertices.NextVertex(i);
            if (!MathUtils.IsCollinear(ref a, ref b, ref c, collinearityTolerance))
            {
                vertices2.Add(b);
            }
        }
        return vertices2;
    }

    public static Vertices DouglasPeuckerSimplify(Vertices vertices, float distanceTolerance)
    {
        if (vertices.Count <= 3)
        {
            return vertices;
        }
        bool[] array = new bool[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            array[i] = true;
        }
        SimplifySection(vertices, 0, vertices.Count - 1, array, distanceTolerance);
        Vertices vertices2 = new(vertices.Count);
        for (int j = 0; j < vertices.Count; j++)
        {
            if (array[j])
            {
                vertices2.Add(vertices[j]);
            }
        }
        return vertices2;
    }

    private static void SimplifySection(Vertices vertices, int i, int j, bool[] usePoint, float distanceTolerance)
    {
        if (i + 1 == j)
        {
            return;
        }
        Vector2 start = vertices[i];
        Vector2 end = vertices[j];
        double num = -1.0;
        int num2 = i;
        for (int k = i + 1; k < j; k++)
        {
            Vector2 point = vertices[k];
            double num3 = LineTools.DistanceBetweenPointAndLineSegment(ref point, ref start, ref end);
            if (num3 > num)
            {
                num = num3;
                num2 = k;
            }
        }
        if (num <= (double)distanceTolerance)
        {
            for (int l = i + 1; l < j; l++)
            {
                usePoint[l] = false;
            }
        }
        else
        {
            SimplifySection(vertices, i, num2, usePoint, distanceTolerance);
            SimplifySection(vertices, num2, j, usePoint, distanceTolerance);
        }
    }

    public static Vertices MergeParallelEdges(Vertices vertices, float tolerance)
    {
        if (vertices.Count <= 3)
        {
            return vertices;
        }
        bool[] array = new bool[vertices.Count];
        int num = vertices.Count;
        for (int i = 0; i < vertices.Count; i++)
        {
            int index = (i == 0) ? (vertices.Count - 1) : (i - 1);
            int index2 = i;
            int index3 = (i != vertices.Count - 1) ? (i + 1) : 0;
            float num2 = vertices[index2].X - vertices[index].X;
            float num3 = vertices[index2].Y - vertices[index].Y;
            float num4 = vertices[index3].Y - vertices[index2].X;
            float num5 = vertices[index3].Y - vertices[index2].Y;
            float num6 = (float)Math.Sqrt((num2 * num2) + (num3 * num3));
            float num7 = (float)Math.Sqrt((num4 * num4) + (num5 * num5));
            if ((!(num6 > 0f) || !(num7 > 0f)) && num > 3)
            {
                array[i] = true;
                num--;
            }
            num2 /= num6;
            num3 /= num6;
            num4 /= num7;
            num5 /= num7;
            float value = (num2 * num5) - (num4 * num3);
            float num8 = (num2 * num4) + (num3 * num5);
            if (Math.Abs(value) < tolerance && num8 > 0f && num > 3)
            {
                array[i] = true;
                num--;
            }
            else
            {
                array[i] = false;
            }
        }
        if (num == vertices.Count || num == 0)
        {
            return vertices;
        }
        int num9 = 0;
        Vertices vertices2 = new(num);
        for (int j = 0; j < vertices.Count; j++)
        {
            if (!array[j] && num != 0 && num9 != num)
            {
                vertices2.Add(vertices[j]);
                num9++;
            }
        }
        return vertices2;
    }

    public static Vertices MergeIdenticalPoints(Vertices vertices)
    {
        HashSet<Vector2> hashSet = [.. vertices];
        return new Vertices(hashSet);
    }

    public static Vertices ReduceByDistance(Vertices vertices, float distance)
    {
        if (vertices.Count <= 3)
        {
            return vertices;
        }
        float num = distance * distance;
        Vertices vertices2 = new(vertices.Count);
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 vector = vertices[i];
            Vector2 vector2 = vertices.NextVertex(i);
            if (!((vector2 - vector).LengthSquared() <= num))
            {
                vertices2.Add(vector);
            }
        }
        return vertices2;
    }

    public static Vertices ReduceByNth(Vertices vertices, int nth)
    {
        if (vertices.Count <= 3)
        {
            return vertices;
        }
        if (nth == 0)
        {
            return vertices;
        }
        Vertices vertices2 = new(vertices.Count);
        for (int i = 0; i < vertices.Count; i++)
        {
            if (i % nth != 0)
            {
                vertices2.Add(vertices[i]);
            }
        }
        return vertices2;
    }

    public static Vertices ReduceByArea(Vertices vertices, float areaTolerance)
    {
        if (vertices.Count <= 3)
        {
            return vertices;
        }
        if (areaTolerance < 0f)
        {
            throw new ArgumentOutOfRangeException("areaTolerance", "must be equal to or greater than zero.");
        }
        Vertices vertices2 = new(vertices.Count);
        Vector2 a = vertices[^2];
        Vector2 b = vertices[^1];
        areaTolerance *= 2f;
        int num = 0;
        while (num < vertices.Count)
        {
            Vector2 b2 = (num == vertices.Count - 1) ? vertices2[0] : vertices[num];
            MathUtils.Cross(ref a, ref b, out var c);
            MathUtils.Cross(ref b, ref b2, out var c2);
            MathUtils.Cross(ref a, ref b2, out var c3);
            if (Math.Abs(c3 - (c + c2)) > areaTolerance)
            {
                vertices2.Add(b);
                a = b;
            }
            num++;
            b = b2;
        }
        return vertices2;
    }
}
