using Mokus2D.Graphics;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual
{
    // The mask sprite must have drawn earlier in this frame so its current frame's quad is ready.
    public class AlphaMaskedNode : Node
    {
        private readonly Vertex[] maskVertices = new Vertex[4];

        public Sprite Mask { get; set; }

        protected override void DrawWithChildren()
        {
            if (Mask == null)
            {
                base.DrawWithChildren();
                return;
            }
            Quad quad = (Quad)Mask.Quad;
            maskVertices[0] = quad.LeftTop;
            maskVertices[1] = quad.RightTop;
            maskVertices[2] = quad.LeftBottom;
            maskVertices[3] = quad.RightBottom;
            Drawer.EndDraw();
            Mokus2DGame.Renderer.BeginAlphaMask(maskVertices, Mask.Texture, MatrixCache.GetScreenMatrix(Root.Size));
            try
            {
                base.DrawWithChildren();
            }
            finally
            {
                Drawer.EndDraw();
                Mokus2DGame.Renderer.EndAlphaMask();
            }
        }
    }
}
