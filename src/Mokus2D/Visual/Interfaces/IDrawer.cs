using Mokus2D.Graphics;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;

namespace Mokus2D.Visual.Interfaces
{
    public interface IDrawer
    {
        void Draw(Quad quad);

        void BeginBatch(ITexture texture, SpriteBatchProperties properties);

        void EndDraw();

        void IncreaseNodesDrawnCount();
    }
}
