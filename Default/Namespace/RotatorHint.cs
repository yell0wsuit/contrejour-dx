using System;
using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatorHint : FadeHint
{
    private RotatorBodyClip rotator;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public RotatorHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        rotator = (RotatorBodyClip)FarseerUtil.Query(this.builder.World, this.builder.ToIPhoneVec(this.clip.Position), 3f, typeof(RotatorBodyClip));
        this.clip.Parent.ChangeChildLayer(this.clip, 12);
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
