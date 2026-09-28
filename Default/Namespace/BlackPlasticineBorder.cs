using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class BlackPlasticineBorder : PlasticineBorder
{
    public BlackPlasticineBorder(List<Vector2> initialPolygon)
        : base(initialPolygon)
    {
    }

    public override Color OutColor()
    {
        return CenterColor().ChangeAlpha(0);
    }

    public override float BorderWidth()
    {
        return 4f;
    }

    public override Color InColor()
    {
        return OutColor();
    }

    public override Color CenterColor()
    {
        return PlasticineConstants.BLACK_BORDER_COLOR;
    }
}
