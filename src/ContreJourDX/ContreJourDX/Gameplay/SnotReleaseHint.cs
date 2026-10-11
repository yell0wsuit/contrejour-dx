using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class SnotReleaseHint : SnotLinkHint
    {
        private bool used;

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
        public SnotReleaseHint(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, null, clip, config)
        {
            HasToRun = false;
        }

        public override void Restart()
        {
            base.Restart();
            HasToRun = false;
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
                Snot.ReleaseEvent.AddListener(OnSnotRelease);
            }
        }

        private void OnSnotRelease()
        {
            Snot.ReleaseEvent.RemoveListener(OnSnotRelease);
            Hide(0.5f);
        }
    }
}
