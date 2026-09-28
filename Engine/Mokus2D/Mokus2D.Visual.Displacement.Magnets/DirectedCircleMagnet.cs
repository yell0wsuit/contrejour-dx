using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement.Magnets;

public class DirectedCircleMagnet : CircleMagnet
{
	public Vector2 Direction = Vector2.One;

	public DirectedCircleMagnet(float radius, float power)
		: base(radius, power)
	{
	}

	public DirectedCircleMagnet(float radius)
		: base(radius)
	{
	}

	protected override Vector2 GetForce(Vector2 relativePosition, float powerCoeff, float length)
	{
		return Power * powerCoeff * Direction;
	}
}
