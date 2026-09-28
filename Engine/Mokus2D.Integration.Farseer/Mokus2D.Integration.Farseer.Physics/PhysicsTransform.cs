using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Physics;

public class PhysicsTransform
{
	public float PhysicsToPixels { get; protected set; }

	public PhysicsTransform(float physicsToPixels)
	{
		PhysicsToPixels = physicsToPixels;
	}

	public Vector2 ToPhysics(float x, float y)
	{
		return ToPhysics(new Vector2(x, y));
	}

	public float ToPhysics(float pixels)
	{
		return pixels / PhysicsToPixels;
	}

	public Vector2 ToPhysics(Vector2 pixels)
	{
		return pixels / PhysicsToPixels;
	}

	public float ToPixels(float physicsPosition)
	{
		return physicsPosition * PhysicsToPixels;
	}

	public Vector2 ToPixels(Vector2 physicsPosition)
	{
		return physicsPosition * PhysicsToPixels;
	}
}
