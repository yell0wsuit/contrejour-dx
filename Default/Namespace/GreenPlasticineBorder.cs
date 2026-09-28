using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class GreenPlasticineBorder : BlackPlasticineBorder
{
    public GreenPlasticineBorder(List<Vector2> initialPolygon)
        : base(initialPolygon)
    {
    }

    public override Color CenterColor()
    {
        return ContreJourConstants.GreenLightColor;
    }
}
