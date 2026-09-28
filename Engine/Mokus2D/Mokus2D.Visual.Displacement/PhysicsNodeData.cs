using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement;

internal sealed class PhysicsNodeData(Vector2 defaultPosition)
{
    public Vector2 Velocity;

    public Vector2 Acceleration;

    public Vector2 DefaultPosition = defaultPosition;
}
