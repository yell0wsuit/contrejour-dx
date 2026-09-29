using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public class TempPolygon
{
    public Vector2[] Vertices { get; set; } = new Vector2[Settings.MaxPolygonVertices];

    public Vector2[] Normals { get; set; } = new Vector2[Settings.MaxPolygonVertices];

    public int Count { get; set; }
}
