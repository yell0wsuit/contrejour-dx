using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Config;

public class FarseerConfig
{
	public static readonly FarseerConfig DefaultConfig = new FarseerConfig();

	public float PhysicsToPixels = 30f;

	public float Density = 0.3f;

	public float Restitution;

	public float Friction = 1f;

	public Vector2 Gravity = new Vector2(0f, 10f);
}
