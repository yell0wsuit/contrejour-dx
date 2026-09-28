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

    public readonly float GetValueInRange()
    {
        return Value + (Maths.Random(-1f, 1f) * Offset);
    }

    public readonly bool Equals(RandomRange other)
    {
        return Value.Equals(other.Value) ? Offset.Equals(other.Offset) : false;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        return obj is RandomRange ? Equals((RandomRange)obj) : false;
    }

    public override readonly int GetHashCode()
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
