using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class ScaleMultEffect(Node target, float scaleMult, float seconds) : ScaleEffect(target, seconds, target.ScaleVec * scaleMult, target.ScaleVec)
{
    private float _scaleMult = scaleMult;

    public float ScaleMult
    {
        get => _scaleMult;
        set
        {
            _scaleMult = value;
            ResetOnValue(OffValue * value);
        }
    }

    public override void ResetOffValue(Vector2 value)
    {
        base.ResetOffValue(value);
        ResetOnValue(OffValue * ScaleMult);
    }
}
