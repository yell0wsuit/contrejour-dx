using System;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public static class LineTools
{
	public static float DistanceBetweenPointAndLineSegment(ref Vector2 point, ref Vector2 start, ref Vector2 end)
	{
		if (start == end)
		{
			return Vector2.Distance(point, start);
		}
		Vector2 vector = Vector2.Subtract(end, start);
		Vector2 value = Vector2.Subtract(point, start);
		float num = Vector2.Dot(value, vector);
		if (num <= 0f)
		{
			return Vector2.Distance(point, start);
		}
		float num2 = Vector2.Dot(vector, vector);
		if (num2 <= num)
		{
			return Vector2.Distance(point, end);
		}
		float scaleFactor = num / num2;
		Vector2 value2 = Vector2.Add(start, Vector2.Multiply(vector, scaleFactor));
		return Vector2.Distance(point, value2);
	}

	public static bool LineIntersect2(ref Vector2 a0, ref Vector2 a1, ref Vector2 b0, ref Vector2 b1, out Vector2 intersectionPoint)
	{
		intersectionPoint = Vector2.Zero;
		if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1)
		{
			return false;
		}
		float x = a0.X;
		float y = a0.Y;
		float x2 = a1.X;
		float y2 = a1.Y;
		float x3 = b0.X;
		float y3 = b0.Y;
		float x4 = b1.X;
		float y4 = b1.Y;
		if (Math.Max(x, x2) < Math.Min(x3, x4) || Math.Max(x3, x4) < Math.Min(x, x2))
		{
			return false;
		}
		if (Math.Max(y, y2) < Math.Min(y3, y4) || Math.Max(y3, y4) < Math.Min(y, y2))
		{
			return false;
		}
		float num = (x4 - x3) * (y - y3) - (y4 - y3) * (x - x3);
		float num2 = (x2 - x) * (y - y3) - (y2 - y) * (x - x3);
		float num3 = (y4 - y3) * (x2 - x) - (x4 - x3) * (y2 - y);
		if (Math.Abs(num3) < 1.1920929E-07f)
		{
			return false;
		}
		num /= num3;
		num2 /= num3;
		if (0f < num && num < 1f && 0f < num2 && num2 < 1f)
		{
			intersectionPoint.X = x + num * (x2 - x);
			intersectionPoint.Y = y + num * (y2 - y);
			return true;
		}
		return false;
	}

	public static Vector2 LineIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
	{
		Vector2 zero = Vector2.Zero;
		float num = p2.Y - p1.Y;
		float num2 = p1.X - p2.X;
		float num3 = num * p1.X + num2 * p1.Y;
		float num4 = q2.Y - q1.Y;
		float num5 = q1.X - q2.X;
		float num6 = num4 * q1.X + num5 * q1.Y;
		float num7 = num * num5 - num4 * num2;
		if (!MathUtils.FloatEquals(num7, 0f))
		{
			zero.X = (num5 * num3 - num2 * num6) / num7;
			zero.Y = (num * num6 - num4 * num3) / num7;
		}
		return zero;
	}

	public static bool LineIntersect(ref Vector2 point1, ref Vector2 point2, ref Vector2 point3, ref Vector2 point4, bool firstIsSegment, bool secondIsSegment, out Vector2 point)
	{
		point = default(Vector2);
		float num = point4.Y - point3.Y;
		float num2 = point2.X - point1.X;
		float num3 = point4.X - point3.X;
		float num4 = point2.Y - point1.Y;
		float num5 = num * num2 - num3 * num4;
		if (!(num5 >= -1.1920929E-07f) || !(num5 <= 1.1920929E-07f))
		{
			float num6 = point1.Y - point3.Y;
			float num7 = point1.X - point3.X;
			float num8 = 1f / num5;
			float num9 = num3 * num6 - num * num7;
			num9 *= num8;
			if (!firstIsSegment || (num9 >= 0f && num9 <= 1f))
			{
				float num10 = num2 * num6 - num4 * num7;
				num10 *= num8;
				if ((!secondIsSegment || (num10 >= 0f && num10 <= 1f)) && (num9 != 0f || num10 != 0f))
				{
					point.X = point1.X + num9 * num2;
					point.Y = point1.Y + num9 * num4;
					return true;
				}
			}
		}
		return false;
	}

	public static bool LineIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, bool firstIsSegment, bool secondIsSegment, out Vector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment, secondIsSegment, out intersectionPoint);
	}

	public static bool LineIntersect(ref Vector2 point1, ref Vector2 point2, ref Vector2 point3, ref Vector2 point4, out Vector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment: true, secondIsSegment: true, out intersectionPoint);
	}

	public static bool LineIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, out Vector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment: true, secondIsSegment: true, out intersectionPoint);
	}

	public static Vertices LineSegmentVerticesIntersect(ref Vector2 point1, ref Vector2 point2, Vertices vertices)
	{
		Vertices vertices2 = new Vertices();
		for (int i = 0; i < vertices.Count; i++)
		{
			if (LineIntersect(vertices[i], vertices[vertices.NextIndex(i)], point1, point2, firstIsSegment: true, secondIsSegment: true, out var intersectionPoint))
			{
				vertices2.Add(intersectionPoint);
			}
		}
		return vertices2;
	}

	public static Vertices LineSegmentAABBIntersect(ref Vector2 point1, ref Vector2 point2, AABB aabb)
	{
		return LineSegmentVerticesIntersect(ref point1, ref point2, aabb.Vertices);
	}
}
