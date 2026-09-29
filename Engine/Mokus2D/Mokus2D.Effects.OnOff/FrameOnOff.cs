using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class FrameOnOff : OnOffEffect
{
    protected float OffFrame { get; }

    protected float OnFrame { get; }

    private IAnimatedNode AnimatedTarget => (IAnimatedNode)Target;

    public FrameOnOff(IAnimatedNode target, float offFrame = 0f, float onFrame = 1f)
        : base((Node)target)
    {
        OffFrame = offFrame;
        OnFrame = onFrame;
        SetOff();
    }

    protected override void SetOn()
    {
        AnimatedTarget.GotoAndStop(OnFrame);
    }

    protected override void SetOff()
    {
        AnimatedTarget.GotoAndStop(OffFrame);
    }

    public override void SetOn(bool value)
    {
        base.SetOn(value);
        if (value)
        {
            SetOn();
        }
        else
        {
            SetOff();
        }
    }
}
