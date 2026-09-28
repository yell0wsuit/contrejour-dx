using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Default.Namespace;

public class FadeEffect : TouchEffect
{
    public FadeEffect(TouchSprite _sprite)
        : base(_sprite)
    {
    }

    public FadeEffect(Node node)
        : base(node)
    {
    }

    public override void OnAction(Node node)
    {
        node.Visible = true;
        _ = node.FadeIn(effectTime);
    }

    public override void OffAction(Node node)
    {
        _ = node.FadeOutAndHide(effectTime);
    }
}
