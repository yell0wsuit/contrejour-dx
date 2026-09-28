using System.Collections.Generic;

using FarseerPhysics.Collision;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.TextureTools;

public static class MarchingSquares
{
    internal class CxFastList<T>
    {
        private CxFastListNode<T> _head;

        private int _count;

        public CxFastListNode<T> Begin()
        {
            return _head;
        }

        public CxFastListNode<T> End()
        {
            return null;
        }

        public T Front()
        {
            return _head.Elem();
        }

        public CxFastListNode<T> Add(T value)
        {
            CxFastListNode<T> cxFastListNode = new(value);
            if (_head == null)
            {
                cxFastListNode._next = null;
                _head = cxFastListNode;
                _count++;
                return cxFastListNode;
            }
            cxFastListNode._next = _head;
            _head = cxFastListNode;
            _count++;
            return cxFastListNode;
        }

        public bool Remove(T value)
        {
            CxFastListNode<T> cxFastListNode = _head;
            CxFastListNode<T> cxFastListNode2 = _head;
            EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
            if (cxFastListNode != null && value != null)
            {
                do
                {
                    if (equalityComparer.Equals(cxFastListNode._elt, value))
                    {
                        if (cxFastListNode == _head)
                        {
                            _head = cxFastListNode._next;
                            _count--;
                            return true;
                        }
                        cxFastListNode2._next = cxFastListNode._next;
                        _count--;
                        return true;
                    }
                    cxFastListNode2 = cxFastListNode;
                    cxFastListNode = cxFastListNode._next;
                }
                while (cxFastListNode != null);
            }
            return false;
        }

        public CxFastListNode<T> Pop()
        {
            return Erase(null, _head);
        }

        public CxFastListNode<T> Insert(CxFastListNode<T> node, T value)
        {
            if (node == null)
            {
                return Add(value);
            }
            CxFastListNode<T> cxFastListNode = new(value);
            CxFastListNode<T> next = node._next;
            cxFastListNode._next = next;
            node._next = cxFastListNode;
            _count++;
            return cxFastListNode;
        }

        public CxFastListNode<T> Erase(CxFastListNode<T> prev, CxFastListNode<T> node)
        {
            CxFastListNode<T> next = node._next;
            if (prev != null)
            {
                prev._next = next;
            }
            else
            {
                if (_head == null)
                {
                    return null;
                }
                _head = _head._next;
            }
            _count--;
            return next;
        }

        public bool Empty()
        {
            return _head == null;
        }

        public int Size()
        {
            CxFastListNode<T> cxFastListNode = Begin();
            int num = 0;
            do
            {
                num++;
            }
            while (cxFastListNode.Next() != null);
            return num;
        }

        public void Clear()
        {
            CxFastListNode<T> cxFastListNode = _head;
            while (cxFastListNode != null)
            {
                CxFastListNode<T> cxFastListNode2 = cxFastListNode;
                cxFastListNode = cxFastListNode._next;
                cxFastListNode2._next = null;
            }
            _head = null;
            _count = 0;
        }

        public bool Has(T value)
        {
            return Find(value) != null;
        }

        public CxFastListNode<T> Find(T value)
        {
            CxFastListNode<T> cxFastListNode = _head;
            EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
            if (cxFastListNode != null)
            {
                if (value != null)
                {
                    do
                    {
                        if (equalityComparer.Equals(cxFastListNode._elt, value))
                        {
                            return cxFastListNode;
                        }
                        cxFastListNode = cxFastListNode._next;
                    }
                    while (cxFastListNode != _head);
                }
                else
                {
                    do
                    {
                        if (cxFastListNode._elt == null)
                        {
                            return cxFastListNode;
                        }
                        cxFastListNode = cxFastListNode._next;
                    }
                    while (cxFastListNode != _head);
                }
            }
            return null;
        }

        public List<T> GetListOfElements()
        {
            List<T> list = [];
            CxFastListNode<T> cxFastListNode = Begin();
            if (cxFastListNode != null)
            {
                do
                {
                    list.Add(cxFastListNode._elt);
                    cxFastListNode = cxFastListNode._next;
                }
                while (cxFastListNode != null);
            }
            return list;
        }
    }

    internal class CxFastListNode<T>
    {
        internal T _elt;

        internal CxFastListNode<T> _next;

        public CxFastListNode(T obj)
        {
            _elt = obj;
        }

        public T Elem()
        {
            return _elt;
        }

        public CxFastListNode<T> Next()
        {
            return _next;
        }
    }

    internal class GeomPoly
    {
        public int Length;

        public CxFastList<Vector2> Points;

        public GeomPoly()
        {
            Points = new CxFastList<Vector2>();
            Length = 0;
        }
    }

    private class GeomPolyVal
    {
        public int Key;

        public GeomPoly GeomP;

        public GeomPolyVal(GeomPoly geomP, int K)
        {
            GeomP = geomP;
            Key = K;
        }
    }

    private static int[] _lookMarch =
    [
        0, 224, 56, 216, 14, 238, 54, 214, 131, 99,
        187, 91, 141, 109, 181, 85
    ];

    public static List<Vertices> DetectSquares(AABB domain, float cellWidth, float cellHeight, sbyte[,] f, int lerpCount, bool combine)
    {
        CxFastList<GeomPoly> cxFastList = new();
        List<Vertices> list = [];
        int num = (int)(domain.Extents.X * 2f / cellWidth);
        bool flag = num == domain.Extents.X * 2f / cellWidth;
        int num2 = (int)(domain.Extents.Y * 2f / cellHeight);
        bool flag2 = num2 == domain.Extents.Y * 2f / cellHeight;
        if (!flag)
        {
            num++;
        }
        if (!flag2)
        {
            num2++;
        }
        sbyte[,] array = new sbyte[num + 1, num2 + 1];
        GeomPolyVal[,] array2 = new GeomPolyVal[num + 1, num2 + 1];
        for (int i = 0; i < num + 1; i++)
        {
            int num3 = (i != num) ? ((int)((i * cellWidth) + domain.LowerBound.X)) : ((int)domain.UpperBound.X);
            for (int j = 0; j < num2 + 1; j++)
            {
                int num4 = (j != num2) ? ((int)((j * cellHeight) + domain.LowerBound.Y)) : ((int)domain.UpperBound.Y);
                array[i, j] = f[num3, num4];
            }
        }
        for (int k = 0; k < num2; k++)
        {
            float num5 = (k * cellHeight) + domain.LowerBound.Y;
            float y = (k != num2 - 1) ? (num5 + cellHeight) : domain.UpperBound.Y;
            GeomPoly polya = null;
            for (int l = 0; l < num; l++)
            {
                float num6 = (l * cellWidth) + domain.LowerBound.X;
                float x = (l != num - 1) ? (num6 + cellWidth) : domain.UpperBound.X;
                GeomPoly poly = new();
                int num7 = MarchSquare(f, array, ref poly, l, k, num6, num5, x, y, lerpCount);
                if (poly.Length != 0)
                {
                    if (combine && polya != null && (num7 & 9) != 0)
                    {
                        combLeft(ref polya, ref poly);
                        poly = polya;
                    }
                    else
                    {
                        _ = cxFastList.Add(poly);
                    }
                    array2[l, k] = new GeomPolyVal(poly, num7);
                }
                else
                {
                    poly = null;
                }
                polya = poly;
            }
        }
        List<GeomPoly> listOfElements;
        if (!combine)
        {
            listOfElements = cxFastList.GetListOfElements();
            {
                foreach (GeomPoly item in listOfElements)
                {
                    list.Add([.. item.Points.GetListOfElements()]);
                }
                return list;
            }
        }
        for (int m = 1; m < num2; m++)
        {
            int num8 = 0;
            while (num8 < num)
            {
                GeomPolyVal geomPolyVal = array2[num8, m];
                if (geomPolyVal == null)
                {
                    num8++;
                    continue;
                }
                if ((geomPolyVal.Key & 0xC) == 0)
                {
                    num8++;
                    continue;
                }
                GeomPolyVal geomPolyVal2 = array2[num8, m - 1];
                if (geomPolyVal2 == null)
                {
                    num8++;
                    continue;
                }
                if ((geomPolyVal2.Key & 3) == 0)
                {
                    num8++;
                    continue;
                }
                float num9 = (num8 * cellWidth) + domain.LowerBound.X;
                float num10 = (m * cellHeight) + domain.LowerBound.Y;
                CxFastList<Vector2> points = geomPolyVal.GeomP.Points;
                CxFastList<Vector2> points2 = geomPolyVal2.GeomP.Points;
                if (geomPolyVal2.GeomP == geomPolyVal.GeomP)
                {
                    num8++;
                    continue;
                }
                CxFastListNode<Vector2> cxFastListNode = points.Begin();
                while (Square(cxFastListNode.Elem().Y - num10) > 1.1920929E-07f || cxFastListNode.Elem().X < num9)
                {
                    cxFastListNode = cxFastListNode.Next();
                }
                Vector2 b = cxFastListNode.Next().Elem();
                if (Square(b.Y - num10) > 1.1920929E-07f)
                {
                    num8++;
                    continue;
                }
                bool flag3 = true;
                CxFastListNode<Vector2> cxFastListNode2;
                for (cxFastListNode2 = points2.Begin(); cxFastListNode2 != points2.End(); cxFastListNode2 = cxFastListNode2.Next())
                {
                    if (VecDsq(cxFastListNode2.Elem(), b) < 1.1920929E-07f)
                    {
                        flag3 = false;
                        break;
                    }
                }
                if (flag3)
                {
                    num8++;
                    continue;
                }
                CxFastListNode<Vector2> cxFastListNode3 = cxFastListNode.Next().Next();
                if (cxFastListNode3 == points.End())
                {
                    cxFastListNode3 = points.Begin();
                }
                while (cxFastListNode3 != cxFastListNode)
                {
                    cxFastListNode2 = points2.Insert(cxFastListNode2, cxFastListNode3.Elem());
                    cxFastListNode3 = cxFastListNode3.Next();
                    if (cxFastListNode3 == points.End())
                    {
                        cxFastListNode3 = points.Begin();
                    }
                    geomPolyVal2.GeomP.Length++;
                }
                num9 = num8 + 1;
                while (num9 < num)
                {
                    GeomPolyVal geomPolyVal3 = array2[(int)num9, m];
                    if (geomPolyVal3 == null || geomPolyVal3.GeomP != geomPolyVal.GeomP)
                    {
                        num9 += 1f;
                        continue;
                    }
                    geomPolyVal3.GeomP = geomPolyVal2.GeomP;
                    num9 += 1f;
                }
                num9 = num8 - 1;
                while (num9 >= 0f)
                {
                    GeomPolyVal geomPolyVal4 = array2[(int)num9, m];
                    if (geomPolyVal4 == null || geomPolyVal4.GeomP != geomPolyVal.GeomP)
                    {
                        num9 -= 1f;
                        continue;
                    }
                    geomPolyVal4.GeomP = geomPolyVal2.GeomP;
                    num9 -= 1f;
                }
                _ = cxFastList.Remove(geomPolyVal.GeomP);
                geomPolyVal.GeomP = geomPolyVal2.GeomP;
                num8 = (int)((cxFastListNode.Next().Elem().X - domain.LowerBound.X) / cellWidth) + 1;
            }
        }
        listOfElements = cxFastList.GetListOfElements();
        foreach (GeomPoly item2 in listOfElements)
        {
            list.Add([.. item2.Points.GetListOfElements()]);
        }
        return list;
    }

    private static float Lerp(float x0, float x1, float v0, float v1)
    {
        float num = v0 - v1;
        float num2 = (!(num * num < 1.1920929E-07f)) ? (v0 / num) : 0.5f;
        return x0 + (num2 * (x1 - x0));
    }

    private static float Xlerp(float x0, float x1, float y, float v0, float v1, sbyte[,] f, int c)
    {
        float num = Lerp(x0, x1, v0, v1);
        if (c == 0)
        {
            return num;
        }
        sbyte b = f[(int)num, (int)y];
        return v0 * b < 0f ? Xlerp(x0, num, y, v0, b, f, c - 1) : Xlerp(num, x1, y, b, v1, f, c - 1);
    }

    private static float Ylerp(float y0, float y1, float x, float v0, float v1, sbyte[,] f, int c)
    {
        float num = Lerp(y0, y1, v0, v1);
        if (c == 0)
        {
            return num;
        }
        sbyte b = f[(int)x, (int)num];
        return v0 * b < 0f ? Ylerp(y0, num, x, v0, b, f, c - 1) : Ylerp(num, y1, x, b, v1, f, c - 1);
    }

    private static float Square(float x)
    {
        return x * x;
    }

    private static float VecDsq(Vector2 a, Vector2 b)
    {
        Vector2 vector = a - b;
        return (vector.X * vector.X) + (vector.Y * vector.Y);
    }

    private static float VecCross(Vector2 a, Vector2 b)
    {
        return (a.X * b.Y) - (a.Y * b.X);
    }

    private static int MarchSquare(sbyte[,] f, sbyte[,] fs, ref GeomPoly poly, int ax, int ay, float x0, float y0, float x1, float y1, int bin)
    {
        int num = 0;
        sbyte b = fs[ax, ay];
        if (b < 0)
        {
            num |= 8;
        }
        sbyte b2 = fs[ax + 1, ay];
        if (b2 < 0)
        {
            num |= 4;
        }
        sbyte b3 = fs[ax + 1, ay + 1];
        if (b3 < 0)
        {
            num |= 2;
        }
        sbyte b4 = fs[ax, ay + 1];
        if (b4 < 0)
        {
            num |= 1;
        }
        int num2 = _lookMarch[num];
        if (num2 != 0)
        {
            CxFastListNode<Vector2> node = null;
            for (int i = 0; i < 8; i++)
            {
                if ((num2 & (1 << i)) != 0)
                {
                    if (i == 7 && (num2 & 1) == 0)
                    {
                        CxFastList<Vector2> points = poly.Points;
                        Vector2 value = new(x0, Ylerp(y0, y1, x0, b, b4, f, bin));
                        _ = points.Add(value);
                    }
                    else
                    {
                        Vector2 value = i switch
                        {
                            0 => new Vector2(x0, y0),
                            2 => new Vector2(x1, y0),
                            4 => new Vector2(x1, y1),
                            6 => new Vector2(x0, y1),
                            1 => new Vector2(Xlerp(x0, x1, y0, b, b2, f, bin), y0),
                            5 => new Vector2(Xlerp(x0, x1, y1, b4, b3, f, bin), y1),
                            3 => new Vector2(x1, Ylerp(y0, y1, x1, b2, b3, f, bin)),
                            _ => new Vector2(x0, Ylerp(y0, y1, x0, b, b4, f, bin)),
                        };
                        node = poly.Points.Insert(node, value);
                    }
                    poly.Length++;
                }
            }
        }
        return num;
    }

    private static void combLeft(ref GeomPoly polya, ref GeomPoly polyb)
    {
        CxFastList<Vector2> points = polya.Points;
        CxFastList<Vector2> points2 = polyb.Points;
        CxFastListNode<Vector2> cxFastListNode = points.Begin();
        CxFastListNode<Vector2> cxFastListNode2 = points2.Begin();
        Vector2 b = cxFastListNode2.Elem();
        CxFastListNode<Vector2> cxFastListNode3 = null;
        while (cxFastListNode != points.End())
        {
            Vector2 vector = cxFastListNode.Elem();
            if (VecDsq(vector, b) < 1.1920929E-07f)
            {
                if (cxFastListNode3 != null)
                {
                    Vector2 vector2 = cxFastListNode3.Elem();
                    b = cxFastListNode2.Next().Elem();
                    Vector2 a = vector - vector2;
                    Vector2 b2 = b - vector;
                    float num = VecCross(a, b2);
                    if (num * num < 1.1920929E-07f)
                    {
                        _ = points.Erase(cxFastListNode3, cxFastListNode);
                        polya.Length--;
                        cxFastListNode = cxFastListNode3;
                    }
                }
                bool flag = true;
                CxFastListNode<Vector2> cxFastListNode4 = null;
                while (!points2.Empty())
                {
                    Vector2 value = points2.Front();
                    _ = points2.Pop();
                    if (!flag && !points2.Empty())
                    {
                        cxFastListNode = points.Insert(cxFastListNode, value);
                        polya.Length++;
                        cxFastListNode4 = cxFastListNode;
                    }
                    flag = false;
                }
                cxFastListNode = cxFastListNode.Next();
                Vector2 vector3 = cxFastListNode.Elem();
                cxFastListNode = cxFastListNode.Next();
                if (cxFastListNode == points.End())
                {
                    cxFastListNode = points.Begin();
                }
                Vector2 vector4 = cxFastListNode.Elem();
                Vector2 vector5 = cxFastListNode4.Elem();
                Vector2 a2 = vector3 - vector5;
                Vector2 b3 = vector4 - vector3;
                float num2 = VecCross(a2, b3);
                if (num2 * num2 < 1.1920929E-07f)
                {
                    _ = points.Erase(cxFastListNode4, cxFastListNode4.Next());
                    polya.Length--;
                }
                break;
            }
            cxFastListNode3 = cxFastListNode;
            cxFastListNode = cxFastListNode.Next();
        }
    }
}
