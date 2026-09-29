using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Config;

public class FarseerConfig
{
    public static readonly FarseerConfig DefaultConfig = new();

    public float PhysicsToPixels { get; set; } = 30f;
    public float Friction { get; set; } = 1f;

}
