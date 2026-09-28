using System;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public struct MassData : IEquatable<MassData>
{
    public float Area { get; internal set; }

    public Vector2 Centroid { get; internal set; }

    public float Inertia { get; internal set; }

    public float Mass { get; internal set; }

    public static bool operator ==(MassData left, MassData right)
    {
        return left.Area == right.Area && left.Mass == right.Mass && left.Centroid == right.Centroid ? left.Inertia == right.Inertia : false;
    }

    public static bool operator !=(MassData left, MassData right)
    {
        return !(left == right);
    }

    public readonly bool Equals(MassData other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        return (object)obj.GetType() != typeof(MassData) ? false : Equals((MassData)obj);
    }

    public override readonly int GetHashCode()
    {
        int hashCode = Area.GetHashCode();
        hashCode = (hashCode * 397) ^ Centroid.GetHashCode();
        hashCode = (hashCode * 397) ^ Inertia.GetHashCode();
        return (hashCode * 397) ^ Mass.GetHashCode();
    }
}
