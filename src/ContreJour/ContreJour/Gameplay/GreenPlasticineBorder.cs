using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;

namespace ContreJour.Gameplay
{
    public class GreenPlasticineBorder(List<Vector2> initialPolygon) : BlackPlasticineBorder(initialPolygon)
    {
        public override Color CenterColor()
        {
            return ContreJourConstants.GreenLightColor;
        }
    }
}
