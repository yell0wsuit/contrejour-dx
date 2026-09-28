using System;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetChangeDoubleRadius : MagnetChangeRadius
{
	private readonly float _radiusDiff;

	private readonly DoubleCircleMagnet DoubleCircleMagnet;

	public MagnetChangeDoubleRadius(DoubleCircleMagnet gridMagnet, float timeout, float targetRadius, float radiusDiff)
		: base(gridMagnet, timeout, targetRadius)
	{
		_radiusDiff = radiusDiff;
		DoubleCircleMagnet = gridMagnet;
	}

	protected override void UpdateMagnet(float ratio)
	{
		base.UpdateMagnet(ratio);
		float excludeRadius = Math.Max(0f, DoubleCircleMagnet.Radius - _radiusDiff);
		DoubleCircleMagnet.ExcludeRadius = excludeRadius;
	}
}
