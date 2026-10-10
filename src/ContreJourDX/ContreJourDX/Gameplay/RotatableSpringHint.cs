using System;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class RotatableSpringHint : FadeHint
    {
        private readonly RotatableSpringBodyClip spring;

        public RotatableSpringHint(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            spring = (RotatableSpringBodyClip)FarseerUtil.Query(Builder.World, Builder.ToIPhoneVec(Clip.Position), 3f, typeof(RotatableSpringBodyClip));
            Clip.Parent.ChangeChildLayer(Clip, 12);
        }

        public override bool HasToHide()
        {
            return false;
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (!Hiding && spring != null && (double)Builder.Game.TotalTime > 0.5 && Math.Abs(Maths.PeriodicOffset(spring.Body.Rotation, (float)Math.PI * 2f)) > (float)Math.PI / 4f)
            {
                Hide(Clip.OpacityFloat / 2f);
            }
        }
    }
}
