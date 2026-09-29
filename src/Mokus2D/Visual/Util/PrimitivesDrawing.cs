using Microsoft.Xna.Framework;

using Mokus2D.Graphics;

namespace Mokus2D.Visual.Util
{
    // The matrix and state GraphUtil's draw helpers use. PrimitivesNode sets them before it draws.
    public static class PrimitivesDrawing
    {
        internal static Matrix CurrentMatrix { get; private set; }

        internal static DrawState CurrentState { get; private set; }

        public static void Begin(Matrix matrix, ITexture texture, float opacity)
        {
            CurrentMatrix = matrix;
            CurrentState = new DrawState(texture, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive, opacity);
        }
    }
}
