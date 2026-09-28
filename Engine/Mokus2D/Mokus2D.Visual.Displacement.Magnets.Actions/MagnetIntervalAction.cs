using System;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetIntervalAction : MagnetIntervalActionBase
{
	private readonly Action<float> _action;

	public MagnetIntervalAction(GridMagnetBase gridMagnet, float timeout, Action<float> action)
		: base(gridMagnet, timeout)
	{
		_action = action;
	}

	protected override void UpdateMagnet(float ratio)
	{
		_action(ratio);
	}
}
