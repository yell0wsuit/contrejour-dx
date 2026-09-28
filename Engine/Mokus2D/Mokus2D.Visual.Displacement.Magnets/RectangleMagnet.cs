using System;
using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement.Magnets;

public class RectangleMagnet : GridMagnetBase
{
	private Vector2 _size;

	private float _borderSize;

	public override Vector2 GetForce(Vector2 relativePosition)
	{
		throw new NotImplementedException();
	}
}
