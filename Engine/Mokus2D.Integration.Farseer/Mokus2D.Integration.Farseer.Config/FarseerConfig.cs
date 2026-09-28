using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Config;

public class FarseerConfig
{
    public static readonly FarseerConfig DefaultConfig = new();

    public float PhysicsToPixels = 30f;
    public float Friction = 1f;

}
