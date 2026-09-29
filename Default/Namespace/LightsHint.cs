using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class LightsHint : FadeHint
{
    private static readonly float QueryRadius = 6.6666665f;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public LightsHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        ((EnergyBodyClip)FarseerUtil.Query(this.builder.World, this.builder.ToIPhoneVec(this.clip.Position), QueryRadius, typeof(EnergyBodyClip))).CollectEvent.AddListener(OnEnergyCollected);
    }

    public override bool HasToHide()
    {
        return false;
    }

    private void OnEnergyCollected()
    {
        if (!Hiding)
        {
            Hiding = true;
            Hide(0.5f * clip.OpacityByte / 255f);
        }
    }
}
