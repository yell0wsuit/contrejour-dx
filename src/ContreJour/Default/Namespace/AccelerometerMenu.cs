using Mokus2D;
using Mokus2D.Input;

namespace Default.Namespace;

public class AccelerometerMenu : AccelerometerNode, ITouchListener
{
    protected virtual int Priority => 1;

    public AccelerometerMenu()
    {
        Mokus2DGame.Instance.TouchController.AddListener(this, Priority);
    }

    public static bool IsFastDevice()
    {
        return true;
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Mokus2DGame.Instance.TouchController.RemoveListener(this);
    }
}
