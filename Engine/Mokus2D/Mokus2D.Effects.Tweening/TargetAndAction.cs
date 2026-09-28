using System;

namespace Mokus2D.Effects.Tweening;

public struct TargetAndAction(object target, Action<object> action)
{
	public object Target = target;

	public Action<object> Action = action;

	public void Execute()
	{
		Action(Target);
	}
}
