using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class BridgeHideHint : SuckerHintBase
{
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public BridgeHideHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
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
