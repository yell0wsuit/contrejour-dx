using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace ContreJour.Gameplay;

public class FadeEffect : TouchEffect
{
    public FadeEffect(TouchSprite sprite)
        : base(sprite)
    {
    }

    public FadeEffect(Node node)
        : base(node)
    {
    }

    public override void OnAction(Node node)
    {
        node.Visible = true;
        _ = node.FadeIn(EffectTime);
    }

    public override void OffAction(Node node)
    {
        _ = node.FadeOutAndHide(EffectTime);
    }
}
