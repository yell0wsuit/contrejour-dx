using Mokus2D.Graphics;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace ContreJourDX.Gameplay
{
    public sealed class AmieHint : FadeHint
    {
        private BaloonBodyClip amie;

        protected override float FadeInTime => 0.5f;

        public AmieHint(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            // The web tutorial uses fontSize / 2 without DX's general UI enlargement.
            foreach (Node child in clip.Children)
            {
                if (child is Label label)
                {
                    label.Scale /= 1.4f;
                    label.Color = Color.Black;
                }
            }
        }

        public override bool HasToHide()
        {
            return false;
        }

        public override void Update(float time)
        {
            base.Update(time);
            // Content ordering may construct the hint before its companion.
            if (amie == null && ContreJourDX.Amie != null)
            {
                amie = ContreJourDX.Amie;
                amie.LinkEvent.AddListener(OnLinked);
            }
        }

        public override void Restart()
        {
            base.Restart();
            if (amie != null)
            {
                amie.LinkEvent.RemoveListener(OnLinked);
                amie.LinkEvent.AddListener(OnLinked);
            }
        }

        private void OnLinked()
        {
            amie.LinkEvent.RemoveListener(OnLinked);
            Hiding = true;
            Hide(0.5f);
        }

        public override void Clear()
        {
            amie?.LinkEvent.RemoveListener(OnLinked);
            base.Clear();
        }
    }
}
