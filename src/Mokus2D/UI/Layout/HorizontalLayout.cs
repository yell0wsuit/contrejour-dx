using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.UI.Layout
{
    public class HorizontalLayout(Node container) : LineLayout(container)
    {
        protected override float GetNodeSize(ISizeNode sizeNode)
        {
            return sizeNode.ScaledSize().X;
        }

        protected override void ChangePosition(ref Vector2 currentPosition, float change)
        {
            currentPosition.X += change;
        }
    }
}
