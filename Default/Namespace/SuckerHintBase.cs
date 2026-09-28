using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SuckerHintBase : FadeHint
{
    protected SuckerBodyClip sucker;

    private static readonly float QUERY_RADIUS = 200f * Box2DConfig.DefaultConfig.SizeMultiplier;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SuckerHintBase(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        sucker = (SuckerBodyClip)FarseerUtil.Query(this.builder.World, this.builder.ToIPhoneVec(this.clip.Position), QUERY_RADIUS, typeof(SuckerBodyClip));
    }
}
