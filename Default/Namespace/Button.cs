using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Default.Namespace;

public class Button : TouchSprite
{
    public bool StopEventPropagation;

    protected bool enabled;

    protected Sprite icon;

    protected Sprite pressed;

    protected float realScale;

    protected bool touching;

    public float RealScale
    {
        get => realScale;
        set
        {
            realScale = value;
            Scale = value;
        }
    }

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    public Sprite Icon => icon;

    public Button(string backgroundFile, string _pressedName, string _iconName)
        : base(backgroundFile)
    {
        realScale = 1f;
        enabled = true;
        if (_pressedName != null)
        {
            pressed = new Sprite(_pressedName);
            AddChild(pressed);
            pressed.Visible = false;
            pressed.OpacityByte = 0;
        }
        if (_iconName != null)
        {
            icon = new Sprite(_iconName);
            AddChild(icon);
        }
    }

    public Button(string backgroundFile, string _iconName)
        : this(backgroundFile, "menu/McButtonPressed", _iconName)
    {
    }

    public Button(string _iconName)
        : this("menu/McButtonBackground", _iconName)
    {
    }

    public static Button ButtonBigWithIcon(string _iconName)
    {
        return new Button("menu/McButtonBackgroundBig", "menu/McButtonPressedBig", _iconName);
    }

    public override bool TouchBegin(Touch touch)
    {
        if (!enabled)
        {
            return false;
        }
        if (StopEventPropagation)
        {
            touch.StopPropagation();
        }
        _ = base.TouchBegin(touch);
        touching = true;
        if (pressed != null)
        {
            pressed.Visible = true;
            pressed.Tweener.Stop();
            _ = pressed.FadeIn(0.1f);
        }
        _ = this.ScaleTo(0.1f, realScale * 1.1f);
        return true;
    }

    public override bool TouchOut(Touch touch)
    {
        HidePressed();
        return enabled && base.TouchOut(touch);
    }

    public override void TouchEnd(Touch touch)
    {
        HidePressed();
        if (enabled)
        {
            base.TouchEnd(touch);
            SoundManager.PlaySound("newClip1", 0.7f);
        }
    }

    public void HidePressed()
    {
        if (touching)
        {
            touching = false;
            _ = this.Schedule(0.2f, DoHidePressed);
        }
    }

    private void DoHidePressed()
    {
        if (!touching)
        {
            _ = this.ScaleTo(0.1f, realScale);
            if (pressed != null)
            {
                pressed.Tweener.Stop();
                _ = pressed.FadeOutAndHide(0.3f);
            }
        }
    }

    protected override void OnAddedToStage()
    {
        base.OnAddedToStage();
        Scale = realScale;
    }
}
