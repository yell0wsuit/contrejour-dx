using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public class TempPolygon
{
	public Vector2[] Vertices = new Vector2[Settings.MaxPolygonVertices];

	public Vector2[] Normals = new Vector2[Settings.MaxPolygonVertices];

	public int Count;
}
