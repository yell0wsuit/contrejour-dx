namespace FarseerPhysics.Common.Decomposition.Seidel;

internal class Point
{
	public Point Next;

	public Point Prev;

	public float X;

	public float Y;

	public Point(float x, float y)
	{
		X = x;
		Y = y;
		Next = null;
		Prev = null;
	}

	public static Point operator -(Point p1, Point p2)
	{
		return new Point(p1.X - p2.X, p1.Y - p2.Y);
	}

	public static Point operator +(Point p1, Point p2)
	{
		return new Point(p1.X + p2.X, p1.Y + p2.Y);
	}

	public static Point operator -(Point p1, float f)
	{
		return new Point(p1.X - f, p1.Y - f);
	}

	public static Point operator +(Point p1, float f)
	{
		return new Point(p1.X + f, p1.Y + f);
	}

	public float Cross(Point p)
	{
		return X * p.Y - Y * p.X;
	}

	public float Dot(Point p)
	{
		return X * p.X + Y * p.Y;
	}

	public bool Neq(Point p)
	{
		if (p.X == X)
		{
			return p.Y != Y;
		}
		return true;
	}

	public float Orient2D(Point pb, Point pc)
	{
		float num = X - pc.X;
		float num2 = pb.X - pc.X;
		float num3 = Y - pc.Y;
		float num4 = pb.Y - pc.Y;
		return num * num4 - num3 * num2;
	}
}
