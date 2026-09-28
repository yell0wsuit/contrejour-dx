using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Config;

public class FarseerConfig
{
    public static readonly FarseerConfig DefaultConfig = new();

    public float PhysicsToPixels = 30f;

    public float Density = 0.3f;

    public float Restitution;

    public float Friction = 1f;

    public Vector2 Gravity = new(0f, 10f);
}
