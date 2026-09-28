using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Construction.Processors;

public class SquareProcessor(PhysicsConstructor constructor) : PolygonProcessor(constructor, Coords)
{
    private static readonly Vector2[] Coords =
    [
        new(-5f, -5f),
        new(-5f, 5f),
        new(5f, 5f),
        new(5f, -5f)
    ];
}
