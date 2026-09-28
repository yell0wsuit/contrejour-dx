using Mokus2D.Visual;

namespace Default.Namespace;

public class Portal2Hint : PortalHint
{
    public Portal2Hint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        hasToRun = false;
    }

    public override void Restart()
    {
        base.Restart();
        hasToRun = false;
    }

    public override void OnPortalUse()
    {
        portal.UseEvent.RemoveListener(OnPortalUse);
        Show();
    }

    public override bool HasToHide()
    {
        return true;
    }
}
