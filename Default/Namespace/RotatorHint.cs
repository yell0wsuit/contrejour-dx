using System;
using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatorHint : FadeHint
{
    protected RotatorBodyClip rotator;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public RotatorHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        rotator = (RotatorBodyClip)FarseerUtil.Query(builder.World, builder.ToIPhoneVec(clip.Position), 3f, typeof(RotatorBodyClip));
        clip.Parent.ChangeChildLayer(clip, 12);
    }

    public override bool HasToHide()
    {
        return false;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!hiding && rotator != null && (double)builder.Game.TotalTime > 0.5 && Math.Abs(Maths.PeriodicOffset(rotator.Body.Rotation, (float)Math.PI * 2f)) > (float)Math.PI / 4f)
        {
            Hide();
        }
    }
}
