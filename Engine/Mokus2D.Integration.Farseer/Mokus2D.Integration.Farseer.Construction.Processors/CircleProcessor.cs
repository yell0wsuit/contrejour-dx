using FarseerPhysics.Collision.Shapes;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction.Processors;

public class CircleProcessor : ShapeProcessor
{
    private const float Radius = 5f;

    public CircleProcessor(PhysicsConstructor constructor)
        : base(constructor)
    {
    }

    public override Shape Process(Node item, Vector2 positionOffset)
    {
        Vector2 vector = Constructor.ToPhysics(new Vector2(5f, 0f), item, positionOffset);
        Vector2 vector2 = Constructor.ToPhysics(Vector2.Zero, item, positionOffset);
        CircleShape circleShape = new CircleShape((vector - vector2).Length(), Constructor.GetDensity(item));
        circleShape.Position = vector2;
        return circleShape;
    }
}
