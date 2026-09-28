using Mokus2D.Input;

namespace Mokus2D.Visual.Interactive;

public class TouchNode : Node, ITouchListener
{
	private readonly TouchListenerDecorator _touchDecorator;

	public bool TouchEnabled
	{
		get
		{
			return _touchDecorator.Enabled;
		}
		set
		{
			_touchDecorator.Enabled = value;
		}
	}

	protected TouchNode()
	{
		_touchDecorator = new TouchListenerDecorator(this);
		_touchDecorator.Filter = IsInteractionsEnabled;
	}

	private bool IsInteractionsEnabled()
	{
		if (base.Root != null && base.RootVisible)
		{
			return base.RootInteractionsEnabled;
		}
		return false;
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
}
