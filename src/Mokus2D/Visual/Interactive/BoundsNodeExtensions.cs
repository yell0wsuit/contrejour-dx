using System.Numerics;

using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Interactive
{
    public static class BoundsNodeExtensions
    {
        public static bool ContainsGlobalPosition(this IBoundsNode sprite, Vector2 position)
        {
            Node node = (Node)sprite;
            if (node.Root == null)
            {
                return false;
            }
            Vector2 value = node.GlobalToLocal(position);
            return sprite.Bounds.Contains(value);
        }
    }
}
