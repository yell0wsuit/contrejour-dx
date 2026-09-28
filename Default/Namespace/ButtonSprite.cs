using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Default.Namespace;

public class ButtonSprite : TouchEffect
{
    private float targetScale;

    private readonly float initialScale;

    public float TargetScale
    {
        get => targetScale;
        set => targetScale = value;
    }

    public ButtonSprite(TouchSprite _sprite)
        : base(_sprite)
    {
        initialScale = Node.Scale;
        targetScale = initialScale * 1.1f;
    }

    public override void OnAction(Node node)
    {
        _ = node.ScaleTo(effectTime, targetScale);
    }

    public override void OffAction(Node node)
    {
        _ = node.ScaleTo(effectTime, initialScale);
    }
}
