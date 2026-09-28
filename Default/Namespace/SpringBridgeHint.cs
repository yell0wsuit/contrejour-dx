using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SpringBridgeHint : FadeHint
{
    private static readonly float QUERY_RADIUS = 100f * Box2DConfig.DefaultConfig.SizeMultiplier;

    protected SpringSuckerBodyClip sucker;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SpringBridgeHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        sucker = (SpringSuckerBodyClip)FarseerUtil.Query(builder.World, builder.ToIPhoneVec(clip.Position), QUERY_RADIUS, typeof(SpringSuckerBodyClip));
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
