using Mokus2D.Visual;

namespace Default.Namespace;

public class TimeoutHint : FadeHint
{
    private const float TIMEOUT = 25f;

    protected bool showing;

    public TimeoutHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        hasToRun = false;
    }

    public override void Restart()
    {
        base.Restart();
        hasToRun = false;
        showing = false;
    }

    public override bool HasToHide()
    {
        return false;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!showing && builder.Game.TotalTime > 25f)
        {
            hasToRun = true;
            showing = true;
            Schedule(Hide, 10f);
        }
    }
}
