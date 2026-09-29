using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class ToggleButton : Button
{
    private bool toggle;

    public Sprite ToggleIcon { get; private set; }

    public bool Toggle
    {
        get => toggle;
        set
        {
            if (toggle != value)
            {
                toggle = value;
                RefreshToggle();
            }
        }
    }

    public ToggleButton(string iconName, string toggleName)
        : base(iconName)
    {
        CreateToggle(toggleName);
    }

    public ToggleButton(string backgroundFile, string iconName, string toggleName)
        : base(backgroundFile, "menu/McButtonPressed", iconName)
    {
        CreateToggle(toggleName);
    }

    public void CreateToggle(string toggleName)
    {
        ToggleIcon = new Sprite(toggleName);
        AddChild(ToggleIcon);
        ToggleIcon.Visible = false;
        ToggleIcon.OpacityByte = 0;
    }

    public override bool TouchBegin(Touch touch)
    {
        bool result = base.TouchBegin(touch);
        if (Enabled)
        {
            RefreshToggle();
        }
        return result;
    }

    public override bool TouchOut(Touch touch)
    {
        bool result = base.TouchOut(touch);
        if (Enabled)
        {
            RefreshToggle();
        }
        return result;
    }

    public override void TouchEnd(Touch touch)
    {
        if (Enabled)
        {
            toggle = !toggle;
        }
        base.TouchEnd(touch);
        RefreshToggle();
    }

    public void RefreshToggle()
    {
        ToggleIcon.Tweener.Stop();
        if (toggle || Touching)
        {
            ToggleIcon.Visible = true;
            _ = ToggleIcon.FadeIn(0.2f);
        }
        else
        {
            _ = ToggleIcon.FadeOutAndHide(0.2f);
        }
    }
}
