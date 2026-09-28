using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class LightsHint : FadeHint
{
    private static readonly float QUERY_RADIUS = 6.6666665f;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public LightsHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        ((EnergyBodyClip)FarseerUtil.Query(builder.World, builder.ToIPhoneVec(clip.Position), QUERY_RADIUS, typeof(EnergyBodyClip))).CollectEvent.AddListener(OnEnergyCollected);
    }

    public override bool HasToHide()
    {
        return false;
    }

    private void OnEnergyCollected()
    {
        if (!hiding)
        {
            hiding = true;
            Hide(0.5f * clip.OpacityByte / 255f);
        }
    }
}
