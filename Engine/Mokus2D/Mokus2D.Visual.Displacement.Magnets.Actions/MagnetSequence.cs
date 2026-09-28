using System.Collections.Generic;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetSequence : MagnetAction
{
	private readonly Queue<MagnetAction> _actions;

	public override bool Finished => _actions.Count == 0;

	public MagnetSequence(params MagnetAction[] actions)
		: base(null)
	{
		_actions = new Queue<MagnetAction>(actions);
	}

	public override void Update(float time)
	{
		base.Update(time);
		MagnetAction magnetAction = _actions.Peek();
		magnetAction.Update(time);
		if (magnetAction.Finished)
		{
			_actions.Dequeue();
		}
	}
}
