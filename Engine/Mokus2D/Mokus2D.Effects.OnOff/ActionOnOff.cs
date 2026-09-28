using System;

namespace Mokus2D.Effects.OnOff;

public class ActionOnOff : IOnOff
{
    private Action<bool> _action;

    private bool _isOn;

    public bool On
    {
        get
        {
            return _isOn;
        }
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                _action(value);
            }
        }
    }

    public ActionOnOff(Action<bool> action)
    {
        _action = action;
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
