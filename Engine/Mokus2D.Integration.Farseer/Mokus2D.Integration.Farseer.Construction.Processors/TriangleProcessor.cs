using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Construction.Processors;

public class TriangleProcessor : PolygonProcessor
{
    private static readonly Vector2[] Coords = new Vector2[3]
    {
        new Vector2(-5f, -5f),
        new Vector2(-5f, 5f),
        new Vector2(5f, -5f)
    };

    public TriangleProcessor(PhysicsConstructor constructor)
        : base(constructor, Coords)
    {
    }
}
