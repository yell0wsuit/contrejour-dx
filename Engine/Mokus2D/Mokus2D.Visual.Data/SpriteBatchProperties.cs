using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Data;

public struct SpriteBatchProperties(BlendState blend, SamplerState samplerState)
{
    public BlendState Blend = blend;

    public SamplerState SamplerState = samplerState;

    public readonly bool Equals(SpriteBatchProperties other)
    {
        return Blend == other.Blend ? Equals(SamplerState, other.SamplerState) : false;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        return (object)obj.GetType() != GetType() ? false : Equals((SpriteBatchProperties)obj);
    }

    public override readonly int GetHashCode()
    {
        return (Blend.GetHashCode() * 397) ^ ((SamplerState != null) ? SamplerState.GetHashCode() : 0);
    }

    public static bool operator ==(SpriteBatchProperties value1, SpriteBatchProperties value2)
    {
        return value1.Blend == value2.Blend ? value1.SamplerState == value2.SamplerState : false;
    }

    public static bool operator !=(SpriteBatchProperties value1, SpriteBatchProperties value2)
    {
        return !(value1 == value2);
    }
}
