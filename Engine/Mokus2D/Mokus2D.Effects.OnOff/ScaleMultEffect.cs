using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class ScaleMultEffect : ScaleEffect
{
    private float _scaleMult;

    public float ScaleMult
    {
        get
        {
            return _scaleMult;
        }
        set
        {
            _scaleMult = value;
            ResetOnValue(base.OffValue * value);
        }
    }

    public ScaleMultEffect(Node target, float scaleMult, float seconds)
        : base(target, seconds, target.ScaleVec * scaleMult, target.ScaleVec)
    {
        _scaleMult = scaleMult;
    }

    public override void ResetOffValue(Vector2 value)
    {
        base.ResetOffValue(value);
        ResetOnValue(base.OffValue * ScaleMult);
    }
}
