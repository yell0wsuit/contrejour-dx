using System;
using Microsoft.Xna.Framework;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Data;

public struct RectangleFloat : IEquatable<RectangleFloat>
{
	private static Rectangle emptyRectangle = default(Rectangle);

	public float X;

	public float Y;

	public float Width;

	public float Height;

	public static Rectangle Empty => emptyRectangle;

	public Vector2 LeftTop => new Vector2(X, Y);

	public Vector2 Size => new Vector2(Width, Height);

	public Vector2 RightBottom => LeftTop + Size;

	public Vector2 LeftBottom => new Vector2(X, Y + Height);

	public Vector2 RightTop => new Vector2(X + Width, Y);

	public float Left => X;

	public float Right => X + Width;

	public float Top => Y;

	public float Bottom => Y + Height;

	public Vector2 Location
	{
		get
		{
			return new Vector2(X, Y);
		}
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	public Vector2 Center => new Vector2(X + Width / 2f, Y + Height / 2f);

	public bool IsEmpty
	{
		get
		{
			if (Width == 0f && Height == 0f && X == 0f)
			{
				return Y == 0f;
			}
			return false;
		}
	}

	public static RectangleFloat Create(Vector2 cornerA, Vector2 cornerB)
	{
		Vector2 vector = Vector2.Min(cornerA, cornerB);
		Vector2 vector2 = Vector2.Max(cornerA, cornerB);
		return new RectangleFloat(vector, vector2 - vector);
	}

	public RectangleFloat(float x, float y, float width, float height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
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
		if (a.X == b.X && a.Y == b.Y && a.Width == b.Width)
		{
			return a.Height == b.Height;
		}
		return false;
	}

	public static RectangleFloat operator *(RectangleFloat a, float mult)
	{
		return new RectangleFloat(a.LeftTop * mult, a.Size * mult);
	}

	public bool Contains(int x, int y)
	{
		if (X <= (float)x && (float)x < X + Width && Y <= (float)y)
		{
			return (float)y < Y + Height;
		}
		return false;
	}

	public bool Contains(float x, float y)
	{
		if (X <= x && x < X + Width && Y <= y)
		{
			return y < Y + Height;
		}
		return false;
	}

	public bool Contains(Point value)
	{
		if (X <= (float)value.X && (float)value.X < X + Width && Y <= (float)value.Y)
		{
			return (float)value.Y < Y + Height;
		}
		return false;
	}

	public bool Contains(Vector2 value)
	{
		if (X <= value.X && value.X < X + Width && Y <= value.Y)
		{
			return value.Y < Y + Height;
		}
		return false;
	}

	public bool Contains(RectangleFloat value)
	{
		if (X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y)
		{
			return value.Y + value.Height <= Y + Height;
		}
		return false;
	}

	public bool Contains(Rectangle value)
	{
		if (X <= (float)value.X && (float)(value.X + value.Width) <= X + Width && Y <= (float)value.Y)
		{
			return (float)(value.Y + value.Height) <= Y + Height;
		}
		return false;
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

	public bool Equals(RectangleFloat other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Rectangle))
		{
			return false;
		}
		return this == (RectangleFloat)obj;
	}

	public override string ToString()
	{
		return string.Format("{{X:{0} Y:{1} Width:{2} Height:{3}}}", new object[4] { X, Y, Width, Height });
	}

	public override int GetHashCode()
	{
		return (int)X ^ (int)Y ^ (int)Width ^ (int)Height;
	}

	public bool Intersects(RectangleFloat value)
	{
		if (value.Left < Right && Left < value.Right && value.Top < Bottom)
		{
			return Top < value.Bottom;
		}
		return false;
	}

	public void Intersects(ref RectangleFloat value, out bool result)
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

	public Vector2 ClampToBounds(Vector2 position)
	{
		return position.Clamp(LeftTop, RightBottom);
	}

	public Vector2 GetOffBoundsOffset(Vector2 position)
	{
		Vector2 vector = ClampToBounds(position);
		return vector - position;
	}

	public static RectangleFloat Intersect(RectangleFloat value1, RectangleFloat value2)
	{
		Intersect(ref value1, ref value2, out var result);
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
