using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class Portal2Hint : PortalHint
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
        public Portal2Hint(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, null, clip, config)
        {
            HasToRun = false;
        }

        public override void Restart()
        {
            base.Restart();
            HasToRun = false;
        }

        public override void OnPortalUse()
        {
            Portal.UseEvent.RemoveListener(OnPortalUse);
            Show();
        }

        public override bool HasToHide()
        {
            return true;
        }
    }
}
