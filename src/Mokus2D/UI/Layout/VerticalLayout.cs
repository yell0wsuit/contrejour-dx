using System.Numerics;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.UI.Layout
{
    public class VerticalLayout(Node container) : LineLayout(container)
    {
        public float TotalHeight => TotalSize;

        protected override float GetNodeSize(ISizeNode sizeNode)
        {
            return sizeNode.ScaledSize().Y;
        }

        protected override void ChangePosition(ref Vector2 currentPosition, float change)
        {
            currentPosition.Y += change;
        }
    }
}
