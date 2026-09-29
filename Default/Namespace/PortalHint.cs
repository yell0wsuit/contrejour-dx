using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class PortalHint : FadeHint
{
    protected TeleportBodyClip Portal { get; set; }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public PortalHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        Restart();
    }

    public override void Restart()
    {
        base.Restart();
        Portal = (TeleportBodyClip)FarseerUtil.Query(builder.World, builder.ToIPhoneVec(clip.Position), 6.6666665f, typeof(TeleportBodyClip));
        Portal.UseEvent.AddListener(OnPortalUse);
    }

    public virtual void OnPortalUse()
    {
        Portal.UseEvent.RemoveListener(OnPortalUse);
        Hide(0.5f * clip.OpacityByte / 255f);
    }

    public override bool HasToHide()
    {
        return false;
    }
}
