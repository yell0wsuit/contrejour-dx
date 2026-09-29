using System;

using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatableSpringHint : FadeHint
{
    private readonly RotatableSpringBodyClip spring;

    public RotatableSpringHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        spring = (RotatableSpringBodyClip)FarseerUtil.Query(this.Builder.World, this.Builder.ToIPhoneVec(this.Clip.Position), 3f, typeof(RotatableSpringBodyClip));
        this.Clip.Parent.ChangeChildLayer(this.Clip, 12);
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
