using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Drawing;

public static class DrawerExtensions
{
	public static void Draw<T>(this IDrawer drawer, T[] vertices, short[] indices) where T : struct, IVertex
	{
		drawer.Draw(vertices, vertices.Length, indices, indices.Length);
	}
}
