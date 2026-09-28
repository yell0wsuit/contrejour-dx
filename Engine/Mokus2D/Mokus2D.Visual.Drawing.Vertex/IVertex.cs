using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Drawing.Vertex;

public interface IVertex : IVertexType
{
	Vector3 Position { get; set; }

	Color Color { get; set; }

	Vector2 TextureCoordinate { get; set; }
}
