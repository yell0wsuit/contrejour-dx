using System;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.Displacement.Magnets;

public class DoubleCircleMagnet : CircleMagnet
{
	public Vector2 ExcludeCircleCenter;

	public float ExcludeRadius;

	public DoubleCircleMagnet(float radius, float excludeRadius, float power)
		: base(radius, power)
	{
		ExcludeRadius = excludeRadius;
	}

	public DoubleCircleMagnet(float radius, float excludeRadius)
		: base(radius)
	{
		ExcludeRadius = excludeRadius;
	}

	public override Vector2 GetForce(Vector2 relativePosition)
	{
		float num = (relativePosition - ExcludeCircleCenter).Length();
		if (num < ExcludeRadius)
		{
			return Vector2.Zero;
		}
		float num2 = relativePosition.Length();
		if (num2 > base.Radius)
		{
			return Vector2.Zero;
		}
		float num3 = Math.Min(num - ExcludeRadius, base.Radius - num2);
		return relativePosition.Normalize(num3 / base.Radius * Power);
	}
}
