using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public class EdgeShape : Shape
{
    internal Vector2 _vertex1;

    internal Vector2 _vertex2;

    public override int ChildCount => 1;

    public bool HasVertex0 { get; set; }

    public bool HasVertex3 { get; set; }

    public Vector2 Vertex0 { get; set; }

    public Vector2 Vertex3 { get; set; }

    public Vector2 Vertex1
    {
        get => _vertex1;
        set
        {
            _vertex1 = value;
            ComputeProperties();
        }
    }

    public Vector2 Vertex2
    {
        get => _vertex2;
        set
        {
            _vertex2 = value;
            ComputeProperties();
        }
    }

    internal EdgeShape()
        : base(0f)
    {
        ShapeType = ShapeType.Edge;
        _radius = 0.01f;
    }

    public EdgeShape(Vector2 start, Vector2 end)
        : base(0f)
    {
        ShapeType = ShapeType.Edge;
        _radius = 0.01f;
        Set(start, end);
    }

    public void Set(Vector2 start, Vector2 end)
    {
        _vertex1 = start;
        _vertex2 = end;
        HasVertex0 = false;
        HasVertex3 = false;
        ComputeProperties();
    }

    public override bool TestPoint(ref Transform transform, ref Vector2 point)
    {
        return false;
    }

    public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
    {
        output = default;
        Vector2 vector = MathUtils.MulT(transform.q, input.Point1 - transform.p);
        Vector2 vector2 = MathUtils.MulT(transform.q, input.Point2 - transform.p);
        Vector2 vector3 = vector2 - vector;
        Vector2 vertex = _vertex1;
        Vector2 vertex2 = _vertex2;
        Vector2 vector4 = vertex2 - vertex;
        Vector2 vector5 = new(vector4.Y, 0f - vector4.X);
        vector5.Normalize();
        float num = Vector2.Dot(vector5, vertex - vector);
        float num2 = Vector2.Dot(vector5, vector3);
        if (num2 == 0f)
        {
            return false;
        }
        float num3 = num / num2;
        if (num3 < 0f || input.MaxFraction < num3)
        {
            return false;
        }
        Vector2 vector6 = vector + (num3 * vector3);
        Vector2 vector7 = vertex2 - vertex;
        float num4 = Vector2.Dot(vector7, vector7);
        if (num4 == 0f)
        {
            return false;
        }
        float num5 = Vector2.Dot(vector6 - vertex, vector7) / num4;
        if (num5 is < 0f or > 1f)
        {
            return false;
        }
        output.Fraction = num3;
        output.Normal = num > 0f ? -vector5 : vector5;
        return true;
    }

    public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
    {
        Vector2 value = MathUtils.Mul(ref transform, _vertex1);
        Vector2 value2 = MathUtils.Mul(ref transform, _vertex2);
        Vector2 vector = Vector2.Min(value, value2);
        Vector2 vector2 = Vector2.Max(value, value2);
        Vector2 vector3 = new(Radius, Radius);
        aabb.LowerBound = vector - vector3;
        aabb.UpperBound = vector2 + vector3;
    }

    protected override void ComputeProperties()
    {
        MassData.Centroid = 0.5f * (_vertex1 + _vertex2);
    }

    public override float ComputeSubmergedArea(ref Vector2 normal, float offset, ref Transform xf, out Vector2 sc)
    {
        sc = Vector2.Zero;
        return 0f;
    }

    public bool CompareTo(EdgeShape shape)
    {
        return HasVertex0 == shape.HasVertex0 && HasVertex3 == shape.HasVertex3 && Vertex0 == shape.Vertex0 && Vertex1 == shape.Vertex1 && Vertex2 == shape.Vertex2
            ? Vertex3 == shape.Vertex3
            : false;
    }

    public override Shape Clone()
    {
        EdgeShape edgeShape = new()
        {
            ShapeType = ShapeType,
            _radius = _radius,
            _density = _density,
            HasVertex0 = HasVertex0,
            HasVertex3 = HasVertex3,
            Vertex0 = Vertex0,
            _vertex1 = _vertex1,
            _vertex2 = _vertex2,
            Vertex3 = Vertex3,
            MassData = MassData
        };
        return edgeShape;
    }
}
