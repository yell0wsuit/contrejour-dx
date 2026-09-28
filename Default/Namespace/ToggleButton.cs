using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class ToggleButton : Button
{
    protected bool toggle;

    protected Sprite toggleIcon;

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

    public ToggleButton(string _iconName, string _toggleName)
        : base(_iconName)
    {
        CreateToggle(_toggleName);
    }

    public ToggleButton(string backgroundFile, string _iconName, string _toggleName)
        : base(backgroundFile, "menu/McButtonPressed", _iconName)
    {
        CreateToggle(_toggleName);
    }

    public void CreateToggle(string _toggleName)
    {
        toggleIcon = new Sprite(_toggleName);
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
