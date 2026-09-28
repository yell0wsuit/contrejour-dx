using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Data;

public struct SpriteBatchProperties(BlendState blend, SamplerState samplerState)
{
    public BlendState Blend = blend;

    public SamplerState SamplerState = samplerState;

    public bool Equals(SpriteBatchProperties other)
    {
        if (Blend == other.Blend)
        {
            return object.Equals(SamplerState, other.SamplerState);
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (object.ReferenceEquals(null, obj))
        {
            return false;
        }
        if (object.ReferenceEquals(this, obj))
        {
            return true;
        }
        if ((object)obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((SpriteBatchProperties)obj);
    }

    public override int GetHashCode()
    {
        return (Blend.GetHashCode() * 397) ^ ((SamplerState != null) ? SamplerState.GetHashCode() : 0);
    }

    public static bool operator ==(SpriteBatchProperties value1, SpriteBatchProperties value2)
    {
        if (value1.Blend == value2.Blend)
        {
            return value1.SamplerState == value2.SamplerState;
        }
        return false;
    }

    public static bool operator !=(SpriteBatchProperties value1, SpriteBatchProperties value2)
    {
        return !(value1 == value2);
    }
}
