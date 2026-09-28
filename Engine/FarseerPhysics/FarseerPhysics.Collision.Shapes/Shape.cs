using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public abstract class Shape
{
    internal float _density;

    internal float _radius;

    internal float _2radius;

    public MassData MassData;

    public ShapeType ShapeType { get; internal set; }

    public abstract int ChildCount { get; }

    public float Density
    {
        get => _density;
        set
        {
            _density = value;
            ComputeProperties();
        }
    }

    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            _2radius = _radius * _radius;
            ComputeProperties();
        }
    }

    protected Shape(float density)
    {
        _density = density;
        ShapeType = ShapeType.Unknown;
    }

    public abstract Shape Clone();

    public abstract bool TestPoint(ref Transform transform, ref Vector2 point);

    public abstract bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex);

    public abstract void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex);

    protected abstract void ComputeProperties();

    public bool CompareTo(Shape shape)
    {
        return shape is PolygonShape && this is PolygonShape
            ? ((PolygonShape)this).CompareTo((PolygonShape)shape)
            : shape is CircleShape && this is CircleShape
            ? ((CircleShape)this).CompareTo((CircleShape)shape)
            : shape is EdgeShape && this is EdgeShape
            ? ((EdgeShape)this).CompareTo((EdgeShape)shape)
            : shape is ChainShape && this is ChainShape && ((ChainShape)this).CompareTo((ChainShape)shape);
    }

    public abstract float ComputeSubmergedArea(ref Vector2 normal, float offset, ref Transform xf, out Vector2 sc);
}
