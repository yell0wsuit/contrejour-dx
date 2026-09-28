using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public class CircleShape : Shape
{
	internal Vector2 _position;

	public override int ChildCount => 1;

	public Vector2 Position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
			ComputeProperties();
		}
	}

	public CircleShape(float radius, float density)
		: base(density)
	{
		base.ShapeType = ShapeType.Circle;
		_position = Vector2.Zero;
		base.Radius = radius;
	}

	internal CircleShape()
		: base(0f)
	{
		base.ShapeType = ShapeType.Circle;
		_radius = 0f;
		_position = Vector2.Zero;
	}

	public override bool TestPoint(ref Transform transform, ref Vector2 point)
	{
		Vector2 vector = transform.p + MathUtils.Mul(transform.q, Position);
		Vector2 vector2 = point - vector;
		return Vector2.Dot(vector2, vector2) <= _2radius;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		output = default(RayCastOutput);
		Vector2 vector = transform.p + MathUtils.Mul(transform.q, Position);
		Vector2 vector2 = input.Point1 - vector;
		float num = Vector2.Dot(vector2, vector2) - _2radius;
		Vector2 vector3 = input.Point2 - input.Point1;
		float num2 = Vector2.Dot(vector2, vector3);
		float num3 = Vector2.Dot(vector3, vector3);
		float num4 = num2 * num2 - num3 * num;
		if (num4 < 0f || num3 < 1.1920929E-07f)
		{
			return false;
		}
		float num5 = 0f - (num2 + (float)Math.Sqrt(num4));
		if (0f <= num5 && num5 <= input.MaxFraction * num3)
		{
			output.Normal = vector2 + (output.Fraction = num5 / num3) * vector3;
			output.Normal.Normalize();
			return true;
		}
		return false;
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		Vector2 vector = transform.p + MathUtils.Mul(transform.q, Position);
		aabb.LowerBound = new Vector2(vector.X - base.Radius, vector.Y - base.Radius);
		aabb.UpperBound = new Vector2(vector.X + base.Radius, vector.Y + base.Radius);
	}

	protected sealed override void ComputeProperties()
	{
		float num = (float)Math.PI * _2radius;
		MassData.Area = num;
		MassData.Mass = base.Density * num;
		MassData.Centroid = Position;
		MassData.Inertia = MassData.Mass * (0.5f * _2radius + Vector2.Dot(Position, Position));
	}

	public override float ComputeSubmergedArea(ref Vector2 normal, float offset, ref Transform xf, out Vector2 sc)
	{
		sc = Vector2.Zero;
		Vector2 vector = MathUtils.Mul(ref xf, Position);
		float num = 0f - (Vector2.Dot(normal, vector) - offset);
		if (num < 0f - base.Radius + 1.1920929E-07f)
		{
			return 0f;
		}
		if (num > base.Radius)
		{
			sc = vector;
			return (float)Math.PI * _2radius;
		}
		float num2 = num * num;
		float num3 = _2radius * (float)(Math.Asin(num / base.Radius) + 1.5707963705062866 + (double)num * Math.Sqrt(_2radius - num2));
		float num4 = -2f / 3f * (float)Math.Pow(_2radius - num2, 1.5) / num3;
		sc.X = vector.X + normal.X * num4;
		sc.Y = vector.Y + normal.Y * num4;
		return num3;
	}

	public bool CompareTo(CircleShape shape)
	{
		if (base.Radius == shape.Radius)
		{
			return Position == shape.Position;
		}
		return false;
	}

	public override Shape Clone()
	{
		CircleShape circleShape = new CircleShape();
		circleShape.ShapeType = base.ShapeType;
		circleShape._radius = base.Radius;
		circleShape._2radius = _2radius;
		circleShape._density = _density;
		circleShape._position = _position;
		circleShape.MassData = MassData;
		return circleShape;
	}
}
