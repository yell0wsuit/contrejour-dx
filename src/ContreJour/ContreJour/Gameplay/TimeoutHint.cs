using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class TimeoutHint : FadeHint
{
    private bool showing;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public TimeoutHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        HasToRun = false;
    }

    public override void Restart()
    {
        base.Restart();
        HasToRun = false;
        showing = false;
    }

    public override bool HasToHide()
    {
        return false;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!showing && Builder.Game.TotalTime > 25f)
        {
            HasToRun = true;
            showing = true;
            Schedule(Hide, 10f);
        }
    }
}
