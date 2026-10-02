using System;

namespace Mokus2D.Util.Data
{
    // An integer rectangle: position of the top-left corner and size, like XNA's.
    public struct Rectangle(int x, int y, int width, int height) : IEquatable<Rectangle>
    {
        public static readonly Rectangle Empty;

        public int X = x;

        public int Y = y;

        public int Width = width;

        public int Height = height;

        public readonly int Left => X;

        public readonly int Top => Y;

        public readonly int Right => X + Width;

        public readonly int Bottom => Y + Height;

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !a.Equals(b);
        }

        public readonly bool Equals(Rectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Rectangle other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }
    }
}
