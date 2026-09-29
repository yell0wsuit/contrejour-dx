using System;
using System.Globalization;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Util.Data
{
    public struct Point(int x, int y)
    {
        public static readonly Point Zero;

        public static readonly Point One = new(1);

        public int X = x;

        public int Y = y;

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
            return new Point((int)(point.X / divider), (int)(point.Y / divider));
        }

        public static Vector2 operator %(Point point, float divider)
        {
            return new Vector2(point.X % divider, point.Y % divider);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public readonly bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override readonly bool Equals(object obj)
        {
            bool result = false;
            if (obj is Point point)
            {
                result = Equals(point);
            }
            return result;
        }

        public override readonly int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override readonly string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[2]
            {
                X.ToString(currentCulture),
                Y.ToString(currentCulture)
            });
        }

        public readonly bool Between(Point a, Point b)
        {
            return X.Between(a.X, b.X) && Y.Between(a.Y, b.Y);
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
}
