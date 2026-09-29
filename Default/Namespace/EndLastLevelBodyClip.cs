using System.Diagnostics.CodeAnalysis;

using Mokus2D.Sound;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class EndLastLevelBodyClip : EndLevelBodyClip
{
    private readonly CosChanger scaleChanger;

    private bool bounce;

    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
    public EndLastLevelBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        scaleChanger = new CosChanger(-0.2f, 0.2f, 0.3f);
    }

    public override void Restart()
    {
        base.Restart();
        Portal.TargetScale = 1f;
        Portal.ScaleStep = 0.05f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (bounce)
        {
            scaleChanger.Update(time);
            Portal.TargetScale = 1.7f + scaleChanger.Value;
        }
    }

    protected override void CompleteLevel(HeroBodyClip bodyClip)
    {
        bodyClip.CompleteLevelSpeed(Body.Position, 0.3f);
        SoundManager.PlaySound("end", 0.5f);
        Portal.TargetScale = 1.7f;
        scaleChanger.SetMiddleProgress();
        scaleChanger.Update(0f);
        Portal.ScaleStep = 0.1f;
        Schedule(Hide, 8f);
    }

    private void Hide()
    {
        bounce = false;
        Portal.TargetScale = 0f;
    }
}
