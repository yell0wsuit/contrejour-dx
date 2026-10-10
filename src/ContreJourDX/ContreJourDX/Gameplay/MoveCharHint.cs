using System;
using System.Numerics;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class MoveCharHint(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config) : FadeHint(builder, body, clip, config)
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
            if (!initialPositionSet && ContreJourDX.Hero != null)
            {
                initialPosition = ContreJourDX.HeroPositionPixels;
                initialPositionSet = true;
            }
            else if (!Hiding && Math.Abs(ContreJourDX.HeroPositionPixels.X - initialPosition.X) > 60f)
            {
                Hide();
            }
        }
    }
}
