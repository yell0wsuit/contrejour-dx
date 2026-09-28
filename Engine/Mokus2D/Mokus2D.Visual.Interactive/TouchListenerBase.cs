using Mokus2D.Input;
using Mokus2D.Util.Resources;

namespace Mokus2D.Visual.Interactive;

public abstract class TouchListenerBase : DisposableBase, ITouchListener
{
    private bool _enabled;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
                if (_enabled)
                {
                    Mokus2DGame.Instance.TouchController.AddListener(this);
                }
                else
                {
                    Mokus2DGame.Instance.TouchController.RemoveListener(this);
                }
                OnEnabledChange();
            }
        }
    }

    protected TouchListenerBase(bool enabled = true)
    {
        Enabled = enabled;
    }

    public abstract bool TouchBegin(Touch touch);

    public abstract bool TouchMove(Touch touch);

    public abstract void TouchEnd(Touch touch);

    protected virtual void OnEnabledChange()
    {
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Enabled = false;
    }
}
