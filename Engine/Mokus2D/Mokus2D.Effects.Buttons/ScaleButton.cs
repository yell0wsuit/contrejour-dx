using Microsoft.Xna.Framework;

using Mokus2D.Effects.OnOff;
using Mokus2D.UI.Controls.Buttons;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Buttons;

public class ScaleButton : Button
{
    public Vector2 DefaultScale
    {
        get
        {
            return ((ScaleMultEffect)Effect).OffValue;
        }
        set
        {
            ((ScaleMultEffect)Effect).ResetOffValue(value);
        }
    }

    public float ScaleMult
    {
        get
        {
            return ((ScaleMultEffect)Effect).ScaleMult;
        }
        set
        {
            ((ScaleMultEffect)Effect).ScaleMult = value;
        }
    }

    public ScaleButton(Sprite target, float targetScale, float seconds)
        : base(target, new ScaleMultEffect(target, targetScale, seconds))
    {
    }

    public ScaleButton(Node effectTarget, AnchorNode clicksTarget, float targetScale, float seconds)
        : base(clicksTarget, new ScaleMultEffect(effectTarget, targetScale, seconds))
    {
    }
}
