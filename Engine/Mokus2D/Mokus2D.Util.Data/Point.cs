using System;
using System.Globalization;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Util.Data;

public struct Point
{
    public static readonly Point Zero = default(Point);

    public static readonly Point One = new Point(1);

    public int X;

    public int Y;

    public static Point Min(Point a, Point b)
    {
        return new Point(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
    }

    public static Point Max(Point a, Point b)
    {
        return new Point(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
    }

    public static Point Clamp(Point point, Point min, Point max)
    {
        point = Max(point, min);
        return Min(point, max);
    }

    public Point(int value)
        : this(value, value)
    {
    }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }

    public static Point operator /(Point point, float divider)
    {
        return new Point((int)((float)point.X / divider), (int)((float)point.Y / divider));
    }

    public static Vector2 operator %(Point point, float divider)
    {
        return new Vector2((float)point.X % divider, (float)point.Y % divider);
    }

    public static bool operator ==(Point a, Point b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Point a, Point b)
    {
        if (a.X == b.X)
        {
            return a.Y != b.Y;
        }
        return true;
    }

    public bool Equals(Point other)
    {
        if (X == other.X)
        {
            return Y == other.Y;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        bool result = false;
        if (obj is Point)
        {
            result = Equals((Point)obj);
        }
        return result;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    public override string ToString()
    {
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[2]
        {
            X.ToString(currentCulture),
            Y.ToString(currentCulture)
        });
    }

    public bool Between(Point a, Point b)
    {
        if (X.Between(a.X, b.X))
        {
            return Y.Between(a.Y, b.Y);
        }
        return false;
    }

    public static implicit operator Vector2(Point src)
    {
        return new Vector2(src.X, src.Y);
    }

    public static explicit operator Point(Vector2 src)
    {
        return new Point((int)src.X, (int)src.Y);
    }
}
