using System.Collections.Generic;
using System.Linq;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class SnotLinkHint(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config) : FadeHint(builder, body, clip, config)
    {
        protected SnotBodyClip Snot { get; set; }

        private bool snotGot;

        public override bool HasToHide()
        {
            return false;
        }

        public override void Restart()
        {
            base.Restart();
            snotGot = false;
        }

        private void GetSnot()
        {
            List<BodyClip> list = FarseerUtil.QueryBodyClipsCenterRadiusType(Builder.World, Builder.ToIPhoneVec(Clip.Position), 6.6666665f, typeof(SnotBodyClip));
            foreach (SnotBodyClip item in list.Cast<SnotBodyClip>())
            {
                item.LinkEvent.AddListener(OnSnotLink);
            }
            Snot = (SnotBodyClip)list[0];
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (Maths.FuzzyNotEquals(time, 0f) && !snotGot)
            {
                GetSnot();
                snotGot = true;
            }
        }

        public virtual void CheckHeroDistance()
        {
        }

        public virtual void OnSnotLink()
        {
            Hiding = true;
            Hide(0.5f * Clip.OpacityByte / 255f);
        }
    }
}
