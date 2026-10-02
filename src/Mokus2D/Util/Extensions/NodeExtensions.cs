using Mokus2D.Graphics;
using Mokus2D.Visual;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Util.Extensions
{
    public static class NodeExtensions
    {

        public static void SetChildrenBlend(this Node node, BlendMode blend, bool recursive = false)
        {
            foreach (Node child in node.Children)
            {
                if (child is IBlendable blendable)
                {
                    blendable.Blend = blend;
                }
                if (recursive)
                {
                    child.SetChildrenBlend(blend, recursive: true);
                }
            }
        }
    }
}
