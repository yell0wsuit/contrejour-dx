using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class Triangulate
{
    private const float EPSILON = 1E-10f;

    public static int Process(List<Vector2> contour, ref List<Vector2> result)
    {
        int count = contour.Count;
        if (count < 3)
        {
            return 0;
        }
        int[] V = new int[count];
        if (0f < Area(contour))
        {
            for (int i = 0; i < count; i++)
            {
                V[i] = i;
            }
        }
        else
        {
            for (int j = 0; j < count; j++)
            {
                V[j] = count - 1 - j;
            }
        }
        int num = count;
        int num2 = 2 * num;
        int result2 = 0;
        int num3 = 0;
        int num4 = num - 1;
        while (num > 2)
        {
            if (0 >= num2--)
            {
                return -1;
            }
            int num5 = num4;
            if (num <= num5)
            {
                num5 = 0;
            }
            num4 = num5 + 1;
            if (num <= num4)
            {
                num4 = 0;
            }
            int num6 = num4 + 1;
            if (num <= num6)
            {
                num6 = 0;
            }
            if (Snip(contour, num5, num4, num6, num, ref V))
            {
                int index = V[num5];
                int index2 = V[num4];
                int index3 = V[num6];
                result[result2++] = contour[index];
                result[result2++] = contour[index2];
                result[result2++] = contour[index3];
                num3++;
                int num7 = num4;
                for (int k = num4 + 1; k < num; k++)
                {
                    V[num7] = V[k];
                    num7++;
                }
                num--;
                num2 = 2 * num;
            }
        }
        V = null;
        return result2;
    }

    public static float Area(List<Vector2> contour)
    {
        int count = contour.Count;
        float num = 0f;
        int index = count - 1;
        int num2 = 0;
        while (num2 < count)
        {
            num += contour[index].X * contour[num2].Y - contour[num2].X * contour[index].Y;
            index = num2++;
        }
        return num * 0.5f;
    }

    public static bool InsideTriangle(float Ax, float Ay, float Bx, float By, float Cx, float Cy, float Px, float Py)
    {
        float num = Cx - Bx;
        float num2 = Cy - By;
        float num3 = Ax - Cx;
        float num4 = Ay - Cy;
        float num5 = Bx - Ax;
        float num6 = By - Ay;
        float num7 = Px - Ax;
        float num8 = Py - Ay;
        float num9 = Px - Bx;
        float num10 = Py - By;
        float num11 = Px - Cx;
        float num12 = Py - Cy;
        float num13 = num * num10 - num2 * num9;
        float num14 = num5 * num8 - num6 * num7;
        float num15 = num3 * num12 - num4 * num11;
        if (num13 >= 0f && num15 >= 0f)
        {
            return num14 >= 0f;
        }
        return false;
    }

    private static bool Snip(List<Vector2> contour, int u, int v, int w, int n, ref int[] V)
    {
        float x = contour[V[u]].X;
        float y = contour[V[u]].Y;
        float x2 = contour[V[v]].X;
        float y2 = contour[V[v]].Y;
        float x3 = contour[V[w]].X;
        float y3 = contour[V[w]].Y;
        if (1E-10f > (x2 - x) * (y3 - y) - (y2 - y) * (x3 - x))
        {
            return false;
        }
        for (int i = 0; i < n; i++)
        {
            if (i != u && i != v && i != w && InsideTriangle(x, y, x2, y2, x3, y3, contour[V[i]].X, contour[V[i]].Y))
            {
                return false;
            }
        }
        return true;
    }
}
