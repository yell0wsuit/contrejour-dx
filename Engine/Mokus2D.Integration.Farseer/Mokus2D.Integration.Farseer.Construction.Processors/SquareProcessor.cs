using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Construction.Processors;

public class SquareProcessor : PolygonProcessor
{
    private static readonly Vector2[] Coords = new Vector2[4]
    {
        new Vector2(-5f, -5f),
        new Vector2(-5f, 5f),
        new Vector2(5f, 5f),
        new Vector2(5f, -5f)
    };

    public SquareProcessor(PhysicsConstructor constructor)
        : base(constructor, Coords)
    {
    }
}
