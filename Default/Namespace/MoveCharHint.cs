using System;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class MoveCharHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config) : FadeHint(builder, body, clip, config)
{
    private Vector2 initialPosition;

    private bool initialPositionSet;

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
