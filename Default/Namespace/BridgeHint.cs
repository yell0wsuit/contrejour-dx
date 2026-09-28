using Mokus2D.Visual;

namespace Default.Namespace;

public class BridgeHint : SuckerHintBase
{
    public BridgeHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
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
