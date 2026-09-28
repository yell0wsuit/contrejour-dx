using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement;

internal sealed class PhysicsNodeData
{
    public Vector2 Velocity;

    public Vector2 Acceleration;

    public Vector2 DefaultPosition;

    public PhysicsNodeData(Vector2 defaultPosition)
    {
        DefaultPosition = defaultPosition;
    }
}
