using System;
using Microsoft.Xna.Framework;
using Mokus2D.Visual;

namespace Default.Namespace;

public class MoveCharHint : FadeHint
{
    private const float MAX_DISTANCE = 60f;

    protected Vector2 initialPosition;

    protected bool initialPositionSet;

    public MoveCharHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
    }

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
