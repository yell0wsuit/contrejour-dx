using Mokus2D.Sound;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class EndLastLevelBodyClip : EndLevelBodyClip
{
    private const float END_SCALE = 1.7f;

    protected CosChanger scaleChanger;

    protected bool bounce;

    public EndLastLevelBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        scaleChanger = new CosChanger(-0.2f, 0.2f, 0.3f);
    }

    public override void Restart()
    {
        base.Restart();
        portal.TargetScale = 1f;
        portal.ScaleStep = 0.05f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (bounce)
        {
            scaleChanger.Update(time);
            portal.TargetScale = 1.7f + scaleChanger.Value;
        }
    }

    protected override void CompleteLevel(HeroBodyClip bodyClip)
    {
        bodyClip.CompleteLevelSpeed(Body.Position, 0.3f);
        SoundManager.PlaySound("end", 0.5f);
        portal.TargetScale = 1.7f;
        scaleChanger.SetMiddleProgress();
        scaleChanger.Update(0f);
        portal.ScaleStep = 0.1f;
        Schedule(Hide, 8f);
    }

    private void Hide()
    {
        bounce = false;
        portal.TargetScale = 0f;
    }
}
