using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class ToggleButton : Button
{
    private bool toggle;

    private Sprite toggleIcon;

    public Sprite ToggleIcon => toggleIcon;

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
        toggleIcon = new Sprite(toggleName);
        AddChild(toggleIcon);
        toggleIcon.Visible = false;
        toggleIcon.OpacityByte = 0;
    }

    public override bool TouchBegin(Touch touch)
    {
        bool result = base.TouchBegin(touch);
        if (enabled)
        {
            RefreshToggle();
        }
        return result;
    }

    public override bool TouchOut(Touch touch)
    {
        bool result = base.TouchOut(touch);
        if (enabled)
        {
            RefreshToggle();
        }
        return result;
    }

    public override void TouchEnd(Touch touch)
    {
        if (enabled)
        {
            toggle = !toggle;
        }
        base.TouchEnd(touch);
        RefreshToggle();
    }

    public void RefreshToggle()
    {
        toggleIcon.Tweener.Stop();
        if (toggle || touching)
        {
            toggleIcon.Visible = true;
            _ = toggleIcon.FadeIn(0.2f);
        }
        else
        {
            _ = toggleIcon.FadeOutAndHide(0.2f);
        }
    }
}
