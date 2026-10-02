using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual
{
    public abstract class PrimitivesNode : Node
    {
        private ITexture texture;

        public virtual ITexture Texture
        {
            get => texture;
            set => texture = value;
        }

        public override void Draw(VisualState state)
        {
            Matrix4x4 combinedScreenMatrix = state.GetCombinedScreenMatrix(Root.Size);
            Drawer.EndDraw();
            PrimitivesDrawing.Begin(combinedScreenMatrix, Texture, state.Opacity);
            DrawPrimitives();
        }

        protected abstract void DrawPrimitives();
    }
}
