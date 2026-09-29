using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace ContreJour.Gameplay;

public class Button : TouchSprite
{
    public bool StopEventPropagation { get; set; }

    public bool Enabled { get; set; }
    private readonly Sprite pressed;

    private float realScale;

    protected bool Touching { get; set; }

    public float RealScale
    {
        get => realScale;
        set
        {
            realScale = value;
            Scale = value;
        }
    }

    public Sprite Icon { get; }

    public Button(string backgroundFile, string pressedName, string iconName)
        : base(backgroundFile)
    {
        realScale = 1f;
        Enabled = true;
        if (pressedName != null)
        {
            pressed = new Sprite(pressedName);
            AddChild(pressed);
            pressed.Visible = false;
            pressed.OpacityByte = 0;
        }
        if (iconName != null)
        {
            Icon = new Sprite(iconName);
            AddChild(Icon);
        }
    }

    public Button(string backgroundFile, string iconName)
        : this(backgroundFile, "menu/McButtonPressed", iconName)
    {
    }

    public Button(string iconName)
        : this("menu/McButtonBackground", iconName)
    {
    }

    public static Button ButtonBigWithIcon(string iconName)
    {
        return new Button("menu/McButtonBackgroundBig", "menu/McButtonPressedBig", iconName);
    }

    public override bool TouchBegin(Touch touch)
    {
        if (!Enabled)
        {
            return false;
        }
        if (StopEventPropagation)
        {
            touch.StopPropagation();
        }
        _ = base.TouchBegin(touch);
        Touching = true;
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
        return Enabled && base.TouchOut(touch);
    }

    public override void TouchEnd(Touch touch)
    {
        HidePressed();
        if (Enabled)
        {
            base.TouchEnd(touch);
            SoundManager.PlaySound("newClip1", 0.7f);
        }
    }

    public void HidePressed()
    {
        if (Touching)
        {
            Touching = false;
            _ = this.Schedule(0.2f, DoHidePressed);
        }
    }

    private void DoHidePressed()
    {
        if (!Touching)
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
