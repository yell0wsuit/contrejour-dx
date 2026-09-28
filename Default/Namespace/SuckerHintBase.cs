using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SuckerHintBase : FadeHint
{
    protected SuckerBodyClip sucker;

    private static readonly float QUERY_RADIUS = 200f * Box2DConfig.DefaultConfig.SizeMultiplier;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SuckerHintBase(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        sucker = (SuckerBodyClip)FarseerUtil.Query(builder.World, builder.ToIPhoneVec(clip.Position), QUERY_RADIUS, typeof(SuckerBodyClip));
    }
}
