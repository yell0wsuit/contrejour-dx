using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class ScaleMultEffect(Node target, float scaleMult, float seconds) : ScaleEffect(target, seconds, target.ScaleVec * scaleMult, target.ScaleVec)
{
    public float ScaleMult
    {
        get;
        set
        {
            field = value;
            ResetOnValue(OffValue * value);
        }
    } = scaleMult;

    public override void ResetOffValue(Vector2 value)
    {
        base.ResetOffValue(value);
        ResetOnValue(OffValue * ScaleMult);
    }
}
