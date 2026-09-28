using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Default.Namespace;

public class ButtonSprite : TouchEffect
{
    private readonly float initialScale;

    public float TargetScale { get; set; }

    public ButtonSprite(TouchSprite sprite)
        : base(sprite)
    {
        initialScale = Node.Scale;
        TargetScale = initialScale * 1.1f;
    }

    public override void OnAction(Node node)
    {
        _ = node.ScaleTo(effectTime, TargetScale);
    }

    public override void OffAction(Node node)
    {
        _ = node.ScaleTo(effectTime, initialScale);
    }
}
