using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.UI.Layout;

public class VerticalLayout : LineLayout
{
    public float TotalHeight => base.TotalSize;

    public VerticalLayout(Node container)
        : base(container)
    {
    }

    protected override float GetNodeSize(ISizeNode sizeNode)
    {
        return sizeNode.ScaledSize().Y;
    }

    protected override void ChangePosition(ref Vector2 currentPosition, float change)
    {
        currentPosition.Y += change;
    }
}
