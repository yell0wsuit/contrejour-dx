using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class GreenPlasticineBorder(List<Vector2> initialPolygon) : BlackPlasticineBorder(initialPolygon)
{
    public override Color CenterColor()
    {
        return ContreJourConstants.GreenLightColor;
    }
}
