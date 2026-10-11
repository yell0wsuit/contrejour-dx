using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class BridgeHideHint : SuckerHintBase
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
        public BridgeHideHint(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, null, clip, config)
        {
            HasToRun = false;
            Sucker.FinishDragEvent.AddListener(OnFinishDrag);
        }

        public override void Restart()
        {
            base.Restart();
            HasToRun = false;
            Sucker.FinishDragEvent.RemoveListener(OnFinishDrag);
            Sucker.RemoveEvent.RemoveListener(OnRemoveBridge);
            Sucker.FinishDragEvent.AddListener(OnFinishDrag);
        }

        public override bool HasToHide()
        {
            return false;
        }

        private void OnFinishDrag()
        {
            HasToRun = true;
            Sucker.FinishDragEvent.RemoveListener(OnFinishDrag);
            Sucker.RemoveEvent.AddListener(OnRemoveBridge);
        }

        private void OnRemoveBridge()
        {
            Sucker.RemoveEvent.RemoveListener(OnRemoveBridge);
            Hide();
        }
    }
}
