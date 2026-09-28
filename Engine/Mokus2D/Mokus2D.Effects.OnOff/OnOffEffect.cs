using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public abstract class OnOffEffect : IOnOff
{
    public bool Test;

    public Node Target;

    private bool _on;

    public bool IsOn
    {
        get => _on;
        set
        {
            if (_on != value)
            {
                _on = value;
                if (_on)
                {
                    SetOn();
                }
                else
                {
                    SetOff();
                }
            }
        }
    }

    protected OnOffEffect(Node target)
    {
        Target = target;
    }

    protected abstract void SetOn();

    protected abstract void SetOff();

    public virtual void SetOn(bool value)
    {
        _on = value;
    }
}
