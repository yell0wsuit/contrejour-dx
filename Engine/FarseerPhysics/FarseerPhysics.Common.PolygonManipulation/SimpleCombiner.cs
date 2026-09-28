using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PolygonManipulation;

public static class SimpleCombiner
{
    public static List<Vertices> PolygonizeTriangles(List<Vertices> triangles, int maxPolys = int.MaxValue, float tolerance = 0.001f)
    {
        if (triangles.Count <= 0)
        {
            return triangles;
        }
        List<Vertices> list = new List<Vertices>();
        bool[] array = new bool[triangles.Count];
        for (int i = 0; i < triangles.Count; i++)
        {
            array[i] = false;
            Vertices vertices = triangles[i];
            Vector2 vector = vertices[0];
            Vector2 vector2 = vertices[1];
            Vector2 vector3 = vertices[2];
            if ((vector.X == vector2.X && vector.Y == vector2.Y) || (vector2.X == vector3.X && vector2.Y == vector3.Y) || (vector.X == vector3.X && vector.Y == vector3.Y))
            {
                array[i] = true;
            }
        }
        int num = 0;
        bool flag = true;
        while (flag)
        {
            int num2 = -1;
            for (int j = 0; j < triangles.Count; j++)
            {
                if (!array[j])
                {
                    num2 = j;
                    break;
                }
            }
            if (num2 == -1)
            {
                flag = false;
                continue;
            }
            Vertices vertices2 = new Vertices(3);
            for (int k = 0; k < 3; k++)
            {
                vertices2.Add(triangles[num2][k]);
            }
            array[num2] = true;
            int num3 = 0;
            int num4 = 0;
            while (num4 < 2 * triangles.Count)
            {
                while (num3 >= triangles.Count)
                {
                    num3 -= triangles.Count;
                }
                if (!array[num3])
                {
                    Vertices vertices3 = AddTriangle(triangles[num3], vertices2);
                    if (vertices3 != null && vertices3.Count <= Settings.MaxPolygonVertices && vertices3.IsConvex())
                    {
                        vertices2 = new Vertices(vertices3);
                        array[num3] = true;
                    }
                }
                num4++;
                num3++;
            }
            if (num < maxPolys)
            {
                SimplifyTools.MergeParallelEdges(vertices2, tolerance);
                if (vertices2.Count >= 3)
                {
                    list.Add(new Vertices(vertices2));
                }
            }
            if (vertices2.Count >= 3)
            {
                num++;
            }
        }
        for (int num5 = list.Count - 1; num5 >= 0; num5--)
        {
            if (list[num5].Count == 0)
            {
                list.RemoveAt(num5);
            }
        }
        return list;
    }

    private static Vertices AddTriangle(Vertices t, Vertices vertices)
    {
        int num = -1;
        int num2 = -1;
        int num3 = -1;
        int num4 = -1;
        for (int i = 0; i < vertices.Count; i++)
        {
            if (t[0].X == vertices[i].X && t[0].Y == vertices[i].Y)
            {
                if (num == -1)
                {
                    num = i;
                    num2 = 0;
                }
                else
                {
                    num3 = i;
                    num4 = 0;
                }
            }
            else if (t[1].X == vertices[i].X && t[1].Y == vertices[i].Y)
            {
                if (num == -1)
                {
                    num = i;
                    num2 = 1;
                }
                else
                {
                    num3 = i;
                    num4 = 1;
                }
            }
            else if (t[2].X == vertices[i].X && t[2].Y == vertices[i].Y)
            {
                if (num == -1)
                {
                    num = i;
                    num2 = 2;
                }
                else
                {
                    num3 = i;
                    num4 = 2;
                }
            }
        }
        if (num == 0 && num3 == vertices.Count - 1)
        {
            num = vertices.Count - 1;
            num3 = 0;
        }
        if (num3 == -1)
        {
            return null;
        }
        int num5 = 0;
        if (num5 == num2 || num5 == num4)
        {
            num5 = 1;
        }
        if (num5 == num2 || num5 == num4)
        {
            num5 = 2;
        }
        Vertices vertices2 = new Vertices(vertices.Count + 1);
        for (int j = 0; j < vertices.Count; j++)
        {
            vertices2.Add(vertices[j]);
            if (j == num)
            {
                vertices2.Add(t[num5]);
            }
        }
        return vertices2;
    }
}
