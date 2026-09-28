using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class BridgeHint : SuckerHintBase
{
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public BridgeHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        sucker.FinishDragEvent.AddListener(OnFinishDrag);
    }

    public override void Restart()
    {
        if (hiding)
        {
            sucker.FinishDragEvent.AddListener(OnFinishDrag);
        }
        base.Restart();
    }

    private void OnFinishDrag()
    {
        sucker.FinishDragEvent.RemoveListener(OnFinishDrag);
        Hide();
    }

    public override bool HasToHide()
    {
        return false;
    }
}
