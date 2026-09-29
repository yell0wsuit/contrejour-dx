using System;
using System.Globalization;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Data
{
    public struct RectangleFloat(float x, float y, float width, float height) : IEquatable<RectangleFloat>
    {
        public float X = x;

        public float Y = y;

        public float Width = width;

        public float Height = height;

        public static Rectangle Empty => default;

        public readonly Vector2 LeftTop => new(X, Y);

        public readonly Vector2 Size => new(Width, Height);

        public readonly Vector2 RightBottom => LeftTop + Size;

        public readonly Vector2 LeftBottom => new(X, Y + Height);

        public readonly Vector2 RightTop => new(X + Width, Y);

        public readonly float Left => X;

        public readonly float Right => X + Width;

        public readonly float Top => Y;

        public readonly float Bottom => Y + Height;

        public Vector2 Location
        {
            readonly get => new(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public readonly Vector2 Center => new(X + (Width / 2f), Y + (Height / 2f));

        public readonly bool IsEmpty => Width == 0f && Height == 0f && X == 0f && Y == 0f;

        public static RectangleFloat Create(Vector2 cornerA, Vector2 cornerB)
        {
            Vector2 vector = Vector2.Min(cornerA, cornerB);
            Vector2 vector2 = Vector2.Max(cornerA, cornerB);
            return new RectangleFloat(vector, vector2 - vector);
        }

        public RectangleFloat(Vector2 position, Vector2 size)
            : this(position.X, position.Y, size.X, size.Y)
        {
        }

        public static implicit operator RectangleFloat(Rectangle rectangle)
        {
            return new RectangleFloat(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static bool operator ==(RectangleFloat a, RectangleFloat b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        }

        public static RectangleFloat operator *(RectangleFloat a, float mult)
        {
            return new RectangleFloat(a.LeftTop * mult, a.Size * mult);
        }

        public readonly bool Contains(int x, int y)
        {
            return X <= x && x < X + Width && Y <= y && y < Y + Height;
        }

        public readonly bool Contains(float x, float y)
        {
            return X <= x && x < X + Width && Y <= y && y < Y + Height;
        }

        public readonly bool Contains(Point value)
        {
            return X <= value.X && value.X < X + Width && Y <= value.Y && value.Y < Y + Height;
        }

        public readonly bool Contains(Vector2 value)
        {
            return X <= value.X && value.X < X + Width && Y <= value.Y && value.Y < Y + Height;
        }

        public readonly bool Contains(RectangleFloat value)
        {
            return X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y && value.Y + value.Height <= Y + Height;
        }

        public readonly bool Contains(Rectangle value)
        {
            return X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y && value.Y + value.Height <= Y + Height;
        }

        public static bool operator !=(RectangleFloat a, RectangleFloat b)
        {
            return !(a == b);
        }

        public void Offset(Vector2 offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Inflate(float horizontalValue, float verticalValue)
        {
            X -= horizontalValue;
            Y -= verticalValue;
            Width += horizontalValue * 2f;
            Height += verticalValue * 2f;
        }

        public readonly bool Equals(RectangleFloat other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Rectangle && this == (RectangleFloat)obj;
        }

        public override readonly string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{X:{0} Y:{1} Width:{2} Height:{3}}}", X, Y, Width, Height);
        }

        public override readonly int GetHashCode()
        {
            return (int)X ^ (int)Y ^ (int)Width ^ (int)Height;
        }

        public readonly bool Intersects(RectangleFloat value)
        {
            return value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom;
        }

        public readonly void Intersects(ref RectangleFloat value, out bool result)
        {
            result = value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom;
        }

        public void Extend(Vector2 value)
        {
            X -= value.X;
            Y -= value.Y;
            Width += value.X * 2f;
            Height += value.Y * 2f;
        }

        public void Extend(float value)
        {
            Extend(new Vector2(value));
        }

        public readonly Vector2 ClampToBounds(Vector2 position)
        {
            return position.Clamp(LeftTop, RightBottom);
        }

        public readonly Vector2 GetOffBoundsOffset(Vector2 position)
        {
            Vector2 vector = ClampToBounds(position);
            return vector - position;
        }

        public static RectangleFloat Intersect(RectangleFloat value1, RectangleFloat value2)
        {
            Intersect(ref value1, ref value2, out RectangleFloat result);
            return result;
        }

        public static void Intersect(ref RectangleFloat value1, ref RectangleFloat value2, out RectangleFloat result)
        {
            if (value1.Intersects(value2))
            {
                float num = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
                float num2 = Math.Max(value1.X, value2.X);
                float num3 = Math.Max(value1.Y, value2.Y);
                float num4 = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
                result = new RectangleFloat(num2, num3, num - num2, num4 - num3);
            }
            else
            {
                result = new RectangleFloat(0f, 0f, 0f, 0f);
            }
        }

        public static RectangleFloat Union(RectangleFloat value1, RectangleFloat value2)
        {
            float num = Math.Min(value1.X, value2.X);
            float num2 = Math.Min(value1.Y, value2.Y);
            return new RectangleFloat(num, num2, Math.Max(value1.Right, value2.Right) - num, Math.Max(value1.Bottom, value2.Bottom) - num2);
        }

        public static void Union(ref RectangleFloat value1, ref RectangleFloat value2, out RectangleFloat result)
        {
            result.X = Math.Min(value1.X, value2.X);
            result.Y = Math.Min(value1.Y, value2.Y);
            result.Width = Math.Max(value1.Right, value2.Right) - result.X;
            result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
        }
    }
}
