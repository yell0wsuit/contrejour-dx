using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class SuckerHintBase : FadeHint
{
    protected SuckerBodyClip Sucker { get; set; }

    private static readonly float QueryRadius = 200f * Box2DConfig.DefaultConfig.SizeMultiplier;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SuckerHintBase(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        Sucker = (SuckerBodyClip)FarseerUtil.Query(Builder.World, Builder.ToIPhoneVec(Clip.Position), QueryRadius, typeof(SuckerBodyClip));
    }
}
