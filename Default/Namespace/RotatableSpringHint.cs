using System;

using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatableSpringHint : FadeHint
{
    private readonly RotatableSpringBodyClip spring;

    public RotatableSpringHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        spring = (RotatableSpringBodyClip)FarseerUtil.Query(this.builder.World, this.builder.ToIPhoneVec(this.clip.Position), 3f, typeof(RotatableSpringBodyClip));
        this.clip.Parent.ChangeChildLayer(this.clip, 12);
    }

    public override bool HasToHide()
    {
        return false;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!Hiding && spring != null && (double)builder.Game.TotalTime > 0.5 && Math.Abs(Maths.PeriodicOffset(spring.Body.Rotation, (float)Math.PI * 2f)) > (float)Math.PI / 4f)
        {
            Hide(clip.OpacityFloat / 2f);
        }
    }
}
