using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SnotReleaseHint : SnotLinkHint
{
    protected bool used;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SnotReleaseHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        hasToRun = false;
    }

    public override void Restart()
    {
        base.Restart();
        hasToRun = false;
        used = false;
    }

    public override bool HasToHide()
    {
        return false;
    }

    public override void CheckHeroDistance()
    {
    }

    public override void OnSnotLink()
    {
        if (!used)
        {
            used = true;
            Show();
            snot.ReleaseEvent.AddListener(OnSnotRelease);
        }
    }

    private void OnSnotRelease()
    {
        snot.ReleaseEvent.RemoveListener(OnSnotRelease);
        Hide(0.5f);
    }
}
