using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Util
{
    public static class TransformationUtil
    {
        public static bool ShouldRefreshNode(Node node)
        {
            return node.Visible && node.OnScreenCount > 0;
        }

        public static void Transform<T>(T[] source, T[] target, ref Matrix matrix) where T : struct, IVertex
        {
            for (int i = 0; i < source.Length; i++)
            {
                Vector2 position = source[i].Position.ToVector2();
                Vector2.Transform(ref position, ref matrix, out Vector2 result);
                target[i].Position = result.ToVector3();
            }
        }
    }
}
