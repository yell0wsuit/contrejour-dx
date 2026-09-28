using Mokus2D.Input;

namespace Mokus2D.Visual.Interactive;

public class TouchNode : Node, ITouchListener
{
    private readonly TouchListenerDecorator _touchDecorator;

    public bool TouchEnabled
    {
        get => _touchDecorator.Enabled;
        set => _touchDecorator.Enabled = value;
    }

    protected TouchNode()
    {
        _touchDecorator = new TouchListenerDecorator(this)
        {
            Filter = IsInteractionsEnabled
        };
    }

    private bool IsInteractionsEnabled()
    {
        return Root != null && RootVisible ? RootInteractionsEnabled : false;
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
