using System;

using Mokus2D.Effects.OnOff;
using Mokus2D.UI.Controls.Buttons;
using Mokus2D.Util;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.UI.Controls.Toggle;

public class ToggleButton : Button
{
    private bool _toggle;

    public bool ToggleOnTouchBegin;

    public bool HighliteOnPressed = true;

    public bool Toggle
    {
        get => _toggle;
        set
        {
            if (_toggle != value)
            {
                _toggle = value;
                RefreshButton();
            }
        }
    }

    public event Action<ToggleButton> ToggleChangeEvent;

    public ToggleButton(AnchorNode content, IOnOff effect)
        : base(content, effect)
    {
    }

    public void ForceEffect()
    {
        Effect.SetOn(Toggle);
    }

    protected override void RefreshButton()
    {
        Effect.On = (Pressed && HighliteOnPressed) || _toggle || MouseOver;
    }

    protected override void OnTouchBegin(TouchArguments touchArguments)
    {
        base.OnTouchBegin(touchArguments);
        if (ToggleOnTouchBegin)
        {
            ChangeToggle();
        }
    }

    protected override void OnTouchEnd(TouchArguments obj)
    {
        if (Pressed)
        {
            if (!ToggleOnTouchBegin)
            {
                ChangeToggle();
            }
            Pressed = false;
        }
        base.OnTouchEnd(obj);
    }

    private void ChangeToggle()
    {
        Toggle = !Toggle;
        ToggleChangeEvent.Dispatch(this);
    }
}
