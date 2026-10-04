using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay
{
    // cj.js outlines the New Friend ground in #FFFB00 while it is dragged.
    public class NewFriendPlasticineBorder(List<Vector2> initialPolygon) : PlasticineBorder(initialPolygon)
    {
        private static readonly Color Yellow = new(255, 251, 0);

        public override Color OutColor()
        {
            return Yellow.ChangeAlpha(0);
        }

        public override Color InColor()
        {
            return OutColor();
        }

        public override Color CenterColor()
        {
            return Yellow;
        }
    }
}
