using System;

namespace Mokus2D.Effects.OnOff;

public class ActionOnOff(Action<bool> action) : IOnOff
{
    private Action<bool> _action = action;

    private bool _isOn;

    public bool IsOn
    {
        get => _isOn;
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                _action(value);
            }
        }
    }

    protected void SetAction(Action<bool> action)
    {
        _action = action;
    }

    public virtual void SetOn(bool value)
    {
        _isOn = value;
    }
}
