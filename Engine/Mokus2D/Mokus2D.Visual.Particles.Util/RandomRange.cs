using System;
using System.Runtime.Serialization;

using Default.Namespace;

namespace Mokus2D.Visual.Particles.Util;

[DataContract]
public struct RandomRange : IEquatable<RandomRange>
{
    [DataMember]
    public float Value;

    [DataMember]
    public float Offset;

    public static RandomRange Create(float min, float max)
    {
        float num = (min + max) / 2f;
        return new RandomRange(num, Math.Abs(max - num));
    }

    public RandomRange(float value, float randomRange)
    {
        Value = value;
        Offset = randomRange;
    }

    public float GetValueInRange()
    {
        return Value + Maths.Random(-1f, 1f) * Offset;
    }

    public bool Equals(RandomRange other)
    {
        if (Value.Equals(other.Value))
        {
            return Offset.Equals(other.Offset);
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (object.ReferenceEquals(null, obj))
        {
            return false;
        }
        if (obj is RandomRange)
        {
            return Equals((RandomRange)obj);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (Value.GetHashCode() * 397) ^ Offset.GetHashCode();
    }

    public static RandomRange operator *(RandomRange value, float mult)
    {
        return new RandomRange(value.Value * mult, value.Offset * mult);
    }

    public static RandomRange operator /(RandomRange value, float mult)
    {
        return new RandomRange(value.Value / mult, value.Offset / mult);
    }
}
