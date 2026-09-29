using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.UI.Layout
{
    public class DefaultPositionApplier : ILayoutPositionApplier
    {
        public void ApplyPosition(Node node, Vector2 position)
        {
            node.Position = position;
        }
    }
}
