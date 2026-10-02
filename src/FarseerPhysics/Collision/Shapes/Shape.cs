using System.Numerics;

using FarseerPhysics.Common;

namespace FarseerPhysics.Collision.Shapes
{
    public abstract class Shape(float density)
    {
        internal float _density = density;

        internal float _radius;

        internal float _2radius;

        private MassData massData;

        public ref MassData MassData => ref massData;

        public ShapeType ShapeType { get; internal set; } = ShapeType.Unknown;

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

        public abstract Shape Clone();

        public abstract bool TestPoint(ref Transform transform, ref Vector2 point);

        public abstract bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex);

        public abstract void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex);

        protected abstract void ComputeProperties();

        public bool CompareTo(Shape shape)
        {
            return shape is PolygonShape otherPolygon && this is PolygonShape polygon
                ? polygon.CompareTo(otherPolygon)
                : shape is CircleShape otherCircle && this is CircleShape circle
                ? circle.CompareTo(otherCircle)
                : shape is EdgeShape otherEdge && this is EdgeShape edge
                ? edge.CompareTo(otherEdge)
                : shape is ChainShape otherChain && this is ChainShape chain && chain.CompareTo(otherChain);
        }

        public abstract float ComputeSubmergedArea(ref Vector2 normal, float offset, ref Transform xf, out Vector2 sc);
    }
}
