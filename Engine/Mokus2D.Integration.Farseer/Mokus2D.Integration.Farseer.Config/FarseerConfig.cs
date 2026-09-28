using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Config;

public class FarseerConfig
{
    public static readonly FarseerConfig DefaultConfig = new();

    public float PhysicsToPixels = 30f;

    private float Density = 0.3f;

    private float Restitution;

    public float Friction = 1f;

    private Vector2 Gravity = new(0f, 10f);
}
