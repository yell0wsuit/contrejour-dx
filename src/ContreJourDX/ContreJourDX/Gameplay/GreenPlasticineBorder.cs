using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;

namespace ContreJourDX.Gameplay
{
    public class GreenPlasticineBorder(List<Vector2> initialPolygon) : BlackPlasticineBorder(initialPolygon)
    {
        public override Color CenterColor()
        {
            return ContreJourDXConstants.GreenLightColor;
        }
    }
}
