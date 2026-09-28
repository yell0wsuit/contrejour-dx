using System;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class MoveCharHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config) : FadeHint(_builder, _body, _clip, _config)
{
    protected Vector2 initialPosition;

    protected bool initialPositionSet;

    public override bool HasToHide()
    {
        return false;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!initialPositionSet && contreJour.Hero != null)
        {
            initialPosition = contreJour.HeroPositionPixels;
            initialPositionSet = true;
        }
        else if (!hiding && Math.Abs(contreJour.HeroPositionPixels.X - initialPosition.X) > 60f)
        {
            Hide();
        }
    }
}
