using Microsoft.Xna.Framework;
using Mokus2D.Visual;

namespace Mokus2D.Parallax;

public class ParallaxLayer
{
	public readonly Node Node;

	public readonly float Parallax;

	public readonly float ParallaxDistance;

	public readonly Vector2 InitialPosition;

	public Vector2 InitialScale;

	public ParallaxLayer(Node node, float parallax)
	{
		Node = node;
		Parallax = parallax;
		InitialPosition = node.Position;
		InitialScale = node.ScaleVec;
		ParallaxDistance = 1f / Parallax;
	}

	public ParallaxLayer(Node node, float parallax, Vector2 initialPosition)
	{
		Node = node;
		Parallax = parallax;
		InitialPosition = initialPosition;
		InitialScale = node.ScaleVec;
		ParallaxDistance = 1f / Parallax;
	}
}
