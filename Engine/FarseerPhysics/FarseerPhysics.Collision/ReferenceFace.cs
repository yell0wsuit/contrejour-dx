using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public struct ReferenceFace
{
	public int i1;

	public int i2;

	public Vector2 v1;

	public Vector2 v2;

	public Vector2 normal;

	public Vector2 sideNormal1;

	public float sideOffset1;

	public Vector2 sideNormal2;

	public float sideOffset2;
}
