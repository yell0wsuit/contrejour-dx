using Microsoft.Xna.Framework;

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
            Matrix combinedScreenMatrix = state.GetCombinedScreenMatrix(Root.Size);
            Drawer.EndDraw();
            using (new PrimitivesDrawing(state, combinedScreenMatrix, Texture))
            {
                DrawPrimitives();
            }
        }

        protected abstract void DrawPrimitives();
    }
}
