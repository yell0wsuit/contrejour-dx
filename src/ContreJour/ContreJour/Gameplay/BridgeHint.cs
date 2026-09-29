using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class BridgeHint : SuckerHintBase
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
        public BridgeHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, null, clip, config)
        {
            Sucker.FinishDragEvent.AddListener(OnFinishDrag);
        }

        public override void Restart()
        {
            if (Hiding)
            {
                Sucker.FinishDragEvent.AddListener(OnFinishDrag);
            }
            base.Restart();
        }

        private void OnFinishDrag()
        {
            Sucker.FinishDragEvent.RemoveListener(OnFinishDrag);
            Hide();
        }

        public override bool HasToHide()
        {
            return false;
        }
    }
}
