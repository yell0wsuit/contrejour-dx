using Mokus2D.Input;

namespace Mokus2D.Visual.Interactive;

public class ClickableLayer : Node, ITouchListener
{
	private readonly int _priority;

	public ClickableLayer(int priority = 0)
	{
		_priority = priority;
	}

	public virtual bool TouchBegin(Touch touch)
	{
		return false;
	}

	public virtual bool TouchMove(Touch touch)
	{
		return false;
	}

	public virtual void TouchEnd(Touch touch)
	{
	}

	protected override void OnAddedToStage()
	{
		base.OnAddedToStage();
		Mokus2DGame.Instance.TouchController.AddListener(this, _priority);
	}

	protected override void OnRemovedFromStage()
	{
		base.OnRemovedFromStage();
		Mokus2DGame.Instance.TouchController.RemoveListener(this);
	}
}
