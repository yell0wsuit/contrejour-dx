using System;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatableSpringHint : FadeHint
{
    private RotatableSpringBodyClip spring;

    public RotatableSpringHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        spring = (RotatableSpringBodyClip)FarseerUtil.Query(builder.World, builder.ToIPhoneVec(clip.Position), 3f, typeof(RotatableSpringBodyClip));
        clip.Parent.ChangeChildLayer(clip, 12);
    }

    public override bool HasToHide()
    {
        return false;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!hiding && spring != null && (double)builder.Game.TotalTime > 0.5 && Math.Abs(Maths.PeriodicOffset(spring.Body.Rotation, (float)Math.PI * 2f)) > (float)Math.PI / 4f)
        {
            Hide(clip.OpacityFloat / 2f);
        }
    }
}
