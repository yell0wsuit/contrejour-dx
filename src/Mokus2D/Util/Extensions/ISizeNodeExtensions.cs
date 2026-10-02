using System.Numerics;

using Mokus2D.Visual;

namespace Mokus2D.Util.Extensions
{
    public static class ISizeNodeExtensions
    {
        public static Vector2 ScaledSize(this ISizeNode node)
        {
            return node.Size * ((Node)node).ScaleVec;
        }

        public static void SetScaledSize(this ISizeNode node, Vector2 value)
        {
            ((Node)node).ScaleVec = value / node.Size;
        }
    }
}
