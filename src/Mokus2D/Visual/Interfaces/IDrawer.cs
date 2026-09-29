using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Interfaces
{
    public interface IDrawer
    {
        void Draw<T>(Quad<T> quad) where T : struct, IVertex;

        void BeginBatch(Texture2D texture, SpriteBatchProperties properties);

        void EndDraw();

        void IncreaseNodesDrawnCount();
    }
}
