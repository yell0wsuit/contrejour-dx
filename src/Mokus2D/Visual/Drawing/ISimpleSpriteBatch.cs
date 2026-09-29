using Microsoft.Xna.Framework;

using Mokus2D.Graphics;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Drawing
{
    public interface ISimpleSpriteBatch
    {
        int TrianglesCount { get; }

        void Begin(ITexture texture, Vector2 screenSize, SpriteBatchProperties properties);

        void Flush();
    }
    public interface ISimpleSpriteBatch<T> : ISimpleSpriteBatch where T : struct, IVertex
    {
        void DrawQuad(Quad<T> quad);
    }
}
