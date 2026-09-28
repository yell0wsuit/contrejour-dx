using FarseerPhysics.Common;
using FarseerPhysics.Common.ConvexHull;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public class PolygonShape : Shape
{
	private Vertices _vertices;

	private Vertices _normals;

	public Vertices Vertices
	{
		get
		{
			return _vertices;
		}
		set
		{
			_vertices = new Vertices(value);
			if (Settings.UseConvexHullPolygons)
			{
				if (_vertices.Count <= 3)
				{
					_vertices.ForceCounterClockWise();
				}
				else
				{
					_vertices = GiftWrap.GetConvexHull(_vertices);
				}
			}
			_normals = new Vertices(_vertices.Count);
			for (int i = 0; i < _vertices.Count; i++)
			{
				int index = ((i + 1 < _vertices.Count) ? (i + 1) : 0);
				Vector2 vector = _vertices[index] - _vertices[i];
				Vector2 item = new Vector2(vector.Y, 0f - vector.X);
				item.Normalize();
				_normals.Add(item);
			}
			ComputeProperties();
		}
	}

	public Vertices Normals => _normals;

	public override int ChildCount => 1;

	public PolygonShape(Vertices vertices, float density)
		: base(density)
	{
		base.ShapeType = ShapeType.Polygon;
		_radius = 0.01f;
		Vertices = vertices;
	}

	public PolygonShape(float density)
		: base(density)
	{
		base.ShapeType = ShapeType.Polygon;
		_radius = 0.01f;
		_vertices = new Vertices(Settings.MaxPolygonVertices);
		_normals = new Vertices(Settings.MaxPolygonVertices);
	}

	internal PolygonShape()
		: base(0f)
	{
		base.ShapeType = ShapeType.Polygon;
		_radius = 0.01f;
		_vertices = new Vertices(Settings.MaxPolygonVertices);
		_normals = new Vertices(Settings.MaxPolygonVertices);
	}

	protected override void ComputeProperties()
	{
		if (!(_density <= 0f))
		{
			Vector2 zero = Vector2.Zero;
			float num = 0f;
			float num2 = 0f;
			Vector2 zero2 = Vector2.Zero;
			for (int i = 0; i < Vertices.Count; i++)
			{
				zero2 += Vertices[i];
			}
			zero2 *= 1f / (float)Vertices.Count;
			for (int j = 0; j < Vertices.Count; j++)
			{
				Vector2 vector = Vertices[j] - zero2;
				Vector2 vector2 = ((j + 1 < Vertices.Count) ? (Vertices[j + 1] - zero2) : (Vertices[0] - zero2));
				float num3 = MathUtils.Cross(vector, vector2);
				float num4 = 0.5f * num3;
				num += num4;
				zero += num4 * (1f / 3f) * (vector + vector2);
				float x = vector.X;
				float y = vector.Y;
				float x2 = vector2.X;
				float y2 = vector2.Y;
				float num5 = x * x + x2 * x + x2 * x2;
				float num6 = y * y + y2 * y + y2 * y2;
				num2 += 1f / 12f * num3 * (num5 + num6);
			}
			MassData.Area = num;
			MassData.Mass = _density * num;
			zero *= 1f / num;
			MassData.Centroid = zero + zero2;
			MassData.Inertia = _density * num2;
			MassData.Inertia += MassData.Mass * (Vector2.Dot(MassData.Centroid, MassData.Centroid) - Vector2.Dot(zero, zero));
		}
	}

	public override bool TestPoint(ref Transform transform, ref Vector2 point)
	{
		Vector2 vector = MathUtils.MulT(transform.q, point - transform.p);
		for (int i = 0; i < Vertices.Count; i++)
		{
			float num = Vector2.Dot(Normals[i], vector - Vertices[i]);
			if (num > 0f)
			{
				return false;
			}
		}
		return true;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		output = default(RayCastOutput);
		Vector2 vector = MathUtils.MulT(transform.q, input.Point1 - transform.p);
		Vector2 vector2 = MathUtils.MulT(transform.q, input.Point2 - transform.p);
		Vector2 value = vector2 - vector;
		float num = 0f;
		float num2 = input.MaxFraction;
		int num3 = -1;
		for (int i = 0; i < Vertices.Count; i++)
		{
			float num4 = Vector2.Dot(Normals[i], Vertices[i] - vector);
			float num5 = Vector2.Dot(Normals[i], value);
			if (num5 == 0f)
			{
				if (num4 < 0f)
				{
					return false;
				}
			}
			else if (num5 < 0f && num4 < num * num5)
			{
				num = num4 / num5;
				num3 = i;
			}
			else if (num5 > 0f && num4 < num2 * num5)
			{
				num2 = num4 / num5;
			}
			if (num2 < num)
			{
				return false;
			}
		}
		if (num3 >= 0)
		{
			output.Fraction = num;
			output.Normal = MathUtils.Mul(transform.q, Normals[num3]);
			return true;
		}
		return false;
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		Vector2 vector = MathUtils.Mul(ref transform, Vertices[0]);
		Vector2 vector2 = vector;
		for (int i = 1; i < Vertices.Count; i++)
		{
			Vector2 value = MathUtils.Mul(ref transform, Vertices[i]);
			vector = Vector2.Min(vector, value);
			vector2 = Vector2.Max(vector2, value);
		}
		Vector2 vector3 = new Vector2(base.Radius, base.Radius);
		aabb.LowerBound = vector - vector3;
		aabb.UpperBound = vector2 + vector3;
	}

	public override float ComputeSubmergedArea(ref Vector2 normal, float offset, ref Transform xf, out Vector2 sc)
	{
		sc = Vector2.Zero;
		Vector2 value = MathUtils.MulT(xf.q, normal);
		float num = offset - Vector2.Dot(normal, xf.p);
		float[] array = new float[Settings.MaxPolygonVertices];
		int num2 = 0;
		int num3 = -1;
		int num4 = -1;
		bool flag = false;
		int i;
		for (i = 0; i < Vertices.Count; i++)
		{
			array[i] = Vector2.Dot(value, Vertices[i]) - num;
			bool flag2 = array[i] < -1.1920929E-07f;
			if (i > 0)
			{
				if (flag2)
				{
					if (!flag)
					{
						num3 = i - 1;
						num2++;
					}
				}
				else if (flag)
				{
					num4 = i - 1;
					num2++;
				}
			}
			flag = flag2;
		}
		switch (num2)
		{
		case 0:
			if (flag)
			{
				sc = MathUtils.Mul(ref xf, MassData.Centroid);
				return MassData.Mass / base.Density;
			}
			return 0f;
		case 1:
			if (num3 == -1)
			{
				num3 = Vertices.Count - 1;
			}
			else
			{
				num4 = Vertices.Count - 1;
			}
			break;
		}
		int num5 = (num3 + 1) % Vertices.Count;
		int num6 = (num4 + 1) % Vertices.Count;
		float num7 = (0f - array[num3]) / (array[num5] - array[num3]);
		float num8 = (0f - array[num4]) / (array[num6] - array[num4]);
		Vector2 vector = new Vector2(Vertices[num3].X * (1f - num7) + Vertices[num5].X * num7, Vertices[num3].Y * (1f - num7) + Vertices[num5].Y * num7);
		Vector2 vector2 = new Vector2(Vertices[num4].X * (1f - num8) + Vertices[num6].X * num8, Vertices[num4].Y * (1f - num8) + Vertices[num6].Y * num8);
		float num9 = 0f;
		Vector2 v = new Vector2(0f, 0f);
		Vector2 vector3 = Vertices[num5];
		i = num5;
		while (i != num6)
		{
			i = (i + 1) % Vertices.Count;
			Vector2 vector4 = ((i != num6) ? Vertices[i] : vector2);
			Vector2 a = vector3 - vector;
			Vector2 b = vector4 - vector;
			float num10 = MathUtils.Cross(a, b);
			float num11 = 0.5f * num10;
			num9 += num11;
			v += num11 * (1f / 3f) * (vector + vector3 + vector4);
			vector3 = vector4;
		}
		v *= 1f / num9;
		sc = MathUtils.Mul(ref xf, v);
		return num9;
	}

	public bool CompareTo(PolygonShape shape)
	{
		if (Vertices.Count != shape.Vertices.Count)
		{
			return false;
		}
		for (int i = 0; i < Vertices.Count; i++)
		{
			if (Vertices[i] != shape.Vertices[i])
			{
				return false;
			}
		}
		if (base.Radius == shape.Radius)
		{
			return MassData == shape.MassData;
		}
		return false;
	}

	public override Shape Clone()
	{
		PolygonShape polygonShape = new PolygonShape();
		polygonShape.ShapeType = base.ShapeType;
		polygonShape._radius = _radius;
		polygonShape._density = _density;
		polygonShape._vertices = new Vertices(_vertices);
		polygonShape._normals = new Vertices(_normals);
		polygonShape.MassData = MassData;
		return polygonShape;
	}
}
