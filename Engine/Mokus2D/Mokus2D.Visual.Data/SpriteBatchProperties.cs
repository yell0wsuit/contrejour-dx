using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Data;

public struct SpriteBatchProperties(BlendState blend, SamplerState samplerState)
{
    public BlendState Blend = blend;

    public SamplerState SamplerState = samplerState;

    public readonly bool Equals(SpriteBatchProperties other)
    {
        return Blend == other.Blend && Equals(SamplerState, other.SamplerState);
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }
        return ReferenceEquals(this, obj) ? true : (object)obj.GetType() == GetType() && Equals((SpriteBatchProperties)obj);
    }

    public override readonly int GetHashCode()
    {
        return (Blend.GetHashCode() * 397) ^ ((SamplerState != null) ? SamplerState.GetHashCode() : 0);
    }

    public static bool operator ==(SpriteBatchProperties value1, SpriteBatchProperties value2)
    {
        return value1.Blend == value2.Blend && value1.SamplerState == value2.SamplerState;
    }

    public static bool operator !=(SpriteBatchProperties value1, SpriteBatchProperties value2)
    {
        return !(value1 == value2);
    }
}
