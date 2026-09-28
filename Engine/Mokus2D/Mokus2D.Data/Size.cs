using Microsoft.Xna.Framework;

namespace Mokus2D.Data;

public readonly struct Size
{
    public readonly int Width;

    public readonly int Height;

    public Size(int size)
        : this(size, size)
    {
    }

    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public static implicit operator Vector2(Size size)
    {
        return new Vector2(size.Width, size.Height);
    }

    public static implicit operator Size(Vector2 vector)
    {
        return new Size((int)vector.X, (int)vector.Y);
    }
}
