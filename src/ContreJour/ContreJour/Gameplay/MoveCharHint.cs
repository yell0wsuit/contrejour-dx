using System;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace ContreJour.Gameplay;

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
        if (!initialPositionSet && ContreJour.Hero != null)
        {
            initialPosition = ContreJour.HeroPositionPixels;
            initialPositionSet = true;
        }
        else if (!Hiding && Math.Abs(ContreJour.HeroPositionPixels.X - initialPosition.X) > 60f)
        {
            Hide();
        }
    }
}
