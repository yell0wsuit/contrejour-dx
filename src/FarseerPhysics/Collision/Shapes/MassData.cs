using System;
using System.Numerics;

namespace FarseerPhysics.Collision.Shapes
{
    public struct MassData : IEquatable<MassData>
    {
        public float Area { get; internal set; }

        public Vector2 Centroid { get; internal set; }

        public float Inertia { get; internal set; }

        public float Mass { get; internal set; }

        public static bool operator ==(MassData left, MassData right)
        {
            return left.Area == right.Area && left.Mass == right.Mass && left.Centroid == right.Centroid && left.Inertia == right.Inertia;
        }

        public static bool operator !=(MassData left, MassData right)
        {
            return !(left == right);
        }

        public readonly bool Equals(MassData other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is not null && (object)obj.GetType() == typeof(MassData) && Equals((MassData)obj);
        }

        public override readonly int GetHashCode()
        {
            int hashCode = Area.GetHashCode();
            hashCode = (hashCode * 397) ^ Centroid.GetHashCode();
            hashCode = (hashCode * 397) ^ Inertia.GetHashCode();
            return (hashCode * 397) ^ Mass.GetHashCode();
        }
    }
}
