using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay
{
    public class BlackPlasticineBorder(List<Vector2> initialPolygon) : PlasticineBorder(initialPolygon)
    {
        public override Color OutColor()
        {
            return CenterColor().ChangeAlpha(0);
        }

        public override Color InColor()
        {
            return OutColor();
        }

        public override Color CenterColor()
        {
            return PlasticineConstants.BlackBorderColor;
        }
    }
}
