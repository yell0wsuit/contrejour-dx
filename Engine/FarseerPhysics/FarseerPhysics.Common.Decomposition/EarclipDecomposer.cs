using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.Decomposition;

internal static class EarclipDecomposer
{
    private class Triangle : Vertices
    {
        public Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float num = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);
            if (num > 0f)
            {
                Add(new Vector2(x1, y1));
                Add(new Vector2(x2, y2));
                Add(new Vector2(x3, y3));
            }
            else
            {
                Add(new Vector2(x1, y1));
                Add(new Vector2(x3, y3));
                Add(new Vector2(x2, y2));
            }
        }

        public bool IsInside(float x, float y)
        {
            Vector2 vector = base[0];
            Vector2 vector2 = base[1];
            Vector2 vector3 = base[2];
            if (x < vector.X && x < vector2.X && x < vector3.X)
            {
                return false;
            }
            if (x > vector.X && x > vector2.X && x > vector3.X)
            {
                return false;
            }
            if (y < vector.Y && y < vector2.Y && y < vector3.Y)
            {
                return false;
            }
            if (y > vector.Y && y > vector2.Y && y > vector3.Y)
            {
                return false;
            }
            float num = x - vector.X;
            float num2 = y - vector.Y;
            float num3 = vector2.X - vector.X;
            float num4 = vector2.Y - vector.Y;
            float num5 = vector3.X - vector.X;
            float num6 = vector3.Y - vector.Y;
            float num7 = num5 * num5 + num6 * num6;
            float num8 = num5 * num3 + num6 * num4;
            float num9 = num5 * num + num6 * num2;
            float num10 = num3 * num3 + num4 * num4;
            float num11 = num3 * num + num4 * num2;
            float num12 = 1f / (num7 * num10 - num8 * num8);
            float num13 = (num10 * num9 - num8 * num11) * num12;
            float num14 = (num7 * num11 - num8 * num9) * num12;
            if (num13 > 0f && num14 > 0f)
            {
                return num13 + num14 < 1f;
            }
            return false;
        }
    }

    public static List<Vertices> ConvexPartition(Vertices vertices, float tolerance = 0.001f)
    {
        return TriangulatePolygon(vertices, tolerance);
    }

    private static List<Vertices> TriangulatePolygon(Vertices vertices, float tolerance)
    {
        if (vertices.Count < 3)
        {
            return new List<Vertices>();
        }
        List<Vertices> list = new List<Vertices>();
        Vertices pin = new Vertices(vertices);
        if (ResolvePinchPoint(pin, out var poutA, out var poutB, tolerance))
        {
            List<Vertices> list2 = TriangulatePolygon(poutA, tolerance);
            List<Vertices> list3 = TriangulatePolygon(poutB, tolerance);
            if (list2.Count == -1 || list3.Count == -1)
            {
                throw new Exception("Can't triangulate your polygon.");
            }
            for (int i = 0; i < list2.Count; i++)
            {
                list.Add(new Vertices(list2[i]));
            }
            for (int j = 0; j < list3.Count; j++)
            {
                list.Add(new Vertices(list3[j]));
            }
            return list;
        }
        Vertices[] array = new Vertices[vertices.Count - 2];
        int num = 0;
        float[] array2 = new float[vertices.Count];
        float[] array3 = new float[vertices.Count];
        for (int k = 0; k < vertices.Count; k++)
        {
            array2[k] = vertices[k].X;
            array3[k] = vertices[k].Y;
        }
        int num2 = vertices.Count;
        while (num2 > 3)
        {
            int num3 = -1;
            float num4 = -10f;
            for (int l = 0; l < num2; l++)
            {
                if (IsEar(l, array2, array3, num2))
                {
                    int num5 = Remainder(l - 1, num2);
                    int num6 = Remainder(l + 1, num2);
                    Vector2 a = new Vector2(array2[num6] - array2[l], array3[num6] - array3[l]);
                    Vector2 b = new Vector2(array2[l] - array2[num5], array3[l] - array3[num5]);
                    Vector2 b2 = new Vector2(array2[num5] - array2[num6], array3[num5] - array3[num6]);
                    a.Normalize();
                    b.Normalize();
                    b2.Normalize();
                    MathUtils.Cross(ref a, ref b, out var c);
                    c = Math.Abs(c);
                    MathUtils.Cross(ref b, ref b2, out var c2);
                    c2 = Math.Abs(c2);
                    MathUtils.Cross(ref b2, ref a, out var c3);
                    c3 = Math.Abs(c3);
                    float num7 = Math.Min(c, Math.Min(c2, c3));
                    if (num7 > num4)
                    {
                        num3 = l;
                        num4 = num7;
                    }
                }
            }
            if (num3 == -1)
            {
                for (int m = 0; m < num; m++)
                {
                    list.Add(array[m]);
                }
                return list;
            }
            num2--;
            float[] array4 = new float[num2];
            float[] array5 = new float[num2];
            int num8 = 0;
            for (int n = 0; n < num2; n++)
            {
                if (num8 == num3)
                {
                    num8++;
                }
                array4[n] = array2[num8];
                array5[n] = array3[num8];
                num8++;
            }
            int num9 = ((num3 == 0) ? num2 : (num3 - 1));
            int num10 = ((num3 != num2) ? (num3 + 1) : 0);
            Triangle triangle = new Triangle(array2[num3], array3[num3], array2[num10], array3[num10], array2[num9], array3[num9]);
            array[num] = triangle;
            num++;
            array2 = array4;
            array3 = array5;
        }
        Triangle triangle2 = new Triangle(array2[1], array3[1], array2[2], array3[2], array2[0], array3[0]);
        array[num] = triangle2;
        num++;
        for (int num11 = 0; num11 < num; num11++)
        {
            list.Add(new Vertices(array[num11]));
        }
        return list;
    }

    private static bool ResolvePinchPoint(Vertices pin, out Vertices poutA, out Vertices poutB, float tolerance)
    {
        poutA = new Vertices();
        poutB = new Vertices();
        if (pin.Count < 3)
        {
            return false;
        }
        bool flag = false;
        int num = -1;
        int num2 = -1;
        for (int i = 0; i < pin.Count; i++)
        {
            for (int j = i + 1; j < pin.Count; j++)
            {
                if (Math.Abs(pin[i].X - pin[j].X) < tolerance && Math.Abs(pin[i].Y - pin[j].Y) < tolerance && j != i + 1)
                {
                    num = i;
                    num2 = j;
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                break;
            }
        }
        if (flag)
        {
            int num3 = num2 - num;
            if (num3 == pin.Count)
            {
                return false;
            }
            for (int k = 0; k < num3; k++)
            {
                int index = Remainder(num + k, pin.Count);
                poutA.Add(pin[index]);
            }
            int num4 = pin.Count - num3;
            for (int l = 0; l < num4; l++)
            {
                int index2 = Remainder(num2 + l, pin.Count);
                poutB.Add(pin[index2]);
            }
        }
        return flag;
    }

    private static int Remainder(int x, int modulus)
    {
        int i;
        for (i = x % modulus; i < 0; i += modulus)
        {
        }
        return i;
    }

    private static bool IsEar(int i, float[] xv, float[] yv, int xvLength)
    {
        if (i >= xvLength || i < 0 || xvLength < 3)
        {
            return false;
        }
        int num = i + 1;
        int num2 = i - 1;
        float num3;
        float num4;
        float num5;
        float num6;
        if (i == 0)
        {
            num3 = xv[0] - xv[xvLength - 1];
            num4 = yv[0] - yv[xvLength - 1];
            num5 = xv[1] - xv[0];
            num6 = yv[1] - yv[0];
            num2 = xvLength - 1;
        }
        else if (i == xvLength - 1)
        {
            num3 = xv[i] - xv[i - 1];
            num4 = yv[i] - yv[i - 1];
            num5 = xv[0] - xv[i];
            num6 = yv[0] - yv[i];
            num = 0;
        }
        else
        {
            num3 = xv[i] - xv[i - 1];
            num4 = yv[i] - yv[i - 1];
            num5 = xv[i + 1] - xv[i];
            num6 = yv[i + 1] - yv[i];
        }
        float num7 = num3 * num6 - num5 * num4;
        if (num7 > 0f)
        {
            return false;
        }
        Triangle triangle = new Triangle(xv[i], yv[i], xv[num], yv[num], xv[num2], yv[num2]);
        for (int j = 0; j < xvLength; j++)
        {
            if (j != i && j != num2 && j != num && triangle.IsInside(xv[j], yv[j]))
            {
                return false;
            }
        }
        return true;
    }
}
