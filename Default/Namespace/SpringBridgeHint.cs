using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SpringBridgeHint : FadeHint
{
    private static readonly float QueryRadius = 100f * Box2DConfig.DefaultConfig.SizeMultiplier;

    private readonly SpringSuckerBodyClip sucker;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SpringBridgeHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        sucker = (SpringSuckerBodyClip)FarseerUtil.Query(this.builder.World, this.builder.ToIPhoneVec(this.clip.Position), QueryRadius, typeof(SpringSuckerBodyClip));
        Restart();
    }

    private void RemoveListeners()
    {
        sucker.ContactEvent.RemoveListener(OnContact);
        sucker.FinishDragEvent.RemoveListener(OnContact);
        sucker.RemoveEvent.RemoveListener(OnContact);
    }

    public override void Restart()
    {
        base.Restart();
        RemoveListeners();
        hasToRun = sucker.Autocreated;
        if (sucker.Autocreated)
        {
            sucker.ContactEvent.AddListener(OnContact);
            sucker.FinishDragEvent.AddListener(OnContact);
            sucker.RemoveEvent.AddListener(OnContact);
        }
    }

    private void OnContact()
    {
        Hide();
        RemoveListeners();
    }

    public override bool HasToHide()
    {
        return false;
    }
}
