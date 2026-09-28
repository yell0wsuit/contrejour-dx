using Mokus2D.Visual;

namespace Default.Namespace;

public class BridgeHideHint : SuckerHintBase
{
    public BridgeHideHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        hasToRun = false;
        sucker.FinishDragEvent.AddListener(OnFinishDrag);
    }

    public override void Restart()
    {
        base.Restart();
        hasToRun = false;
        sucker.FinishDragEvent.RemoveListener(OnFinishDrag);
        sucker.RemoveEvent.RemoveListener(OnRemoveBridge);
        sucker.FinishDragEvent.AddListener(OnFinishDrag);
    }

    public override bool HasToHide()
    {
        return false;
    }

    private void OnFinishDrag()
    {
        hasToRun = true;
        sucker.FinishDragEvent.RemoveListener(OnFinishDrag);
        sucker.RemoveEvent.AddListener(OnRemoveBridge);
    }

    private void OnRemoveBridge()
    {
        sucker.RemoveEvent.RemoveListener(OnRemoveBridge);
        Hide();
    }
}
