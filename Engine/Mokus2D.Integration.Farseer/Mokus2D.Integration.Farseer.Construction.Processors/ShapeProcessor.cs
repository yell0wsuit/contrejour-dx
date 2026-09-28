using FarseerPhysics.Collision.Shapes;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction.Processors;

public abstract class ShapeProcessor(PhysicsConstructor constructor) : PhysicsProcessor(constructor)
{
    public abstract Shape Process(Node item, Vector2 positionOffset);
}
