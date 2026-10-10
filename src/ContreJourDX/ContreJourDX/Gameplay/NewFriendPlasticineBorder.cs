using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;

namespace ContreJourDX.Gameplay
{
    // cj.js strokes the New Friend rest outline as a solid 2px #FFFB00 line.
    public class NewFriendPlasticineBorder(List<Vector2> initialPolygon) : PlasticineBorder(initialPolygon)
    {
        private static readonly Color Yellow = new(255, 251, 0);

        public override float BorderWidth()
        {
            return 1f;
        }

        public override Color OutColor()
        {
            return ScaledCenterColor();
        }

        public override Color InColor()
        {
            return ScaledCenterColor();
        }

        public override Color CenterColor()
        {
            return Yellow;
        }
    }
}
