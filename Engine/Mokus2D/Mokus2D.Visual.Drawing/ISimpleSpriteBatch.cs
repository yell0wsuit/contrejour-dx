using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Drawing;

public interface ISimpleSpriteBatch
{
	int TrianglesCount { get; }

	void Begin(Texture2D texture, Vector2 screenSize, SpriteBatchProperties properties, ISpriteBatchEffect effect);

	void End();
}
public interface ISimpleSpriteBatch<T> : ISimpleSpriteBatch where T : struct, IVertex
{
	void Draw(T[] vertices);

	void Draw(T[] vertices, int verticesCount, short[] indices, int indicesCount);

	void DrawQuad(Quad<T> quad);
}
