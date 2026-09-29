using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RadiusClickListener(Node target, float clickRadius, int priority = 0) : ClickListener(priority)
{
    private readonly float clickRadius = clickRadius;
    private readonly Node target = target;

    public bool DisableDrag { get; set; }

    public override bool Enabled => base.Enabled && target.RootVisible;

    private bool SpriteContainsPoint(Touch touch)
    {
        return target.Root != null && target.GlobalToLocal(touch.Position).Length() < clickRadius;
    }

    public override bool TouchBegin(Touch touch)
    {
        if (SpriteContainsPoint(touch))
        {
            _ = base.TouchBegin(touch);
            return true;
        }
        return false;
    }

    protected override bool IsOutStartPosition(Touch touch, Vector2 startPosition)
    {
        bool flag = !SpriteContainsPoint(touch);
        if (DisableDrag)
        {
            flag |= base.IsOutStartPosition(touch, startPosition);
        }
        return flag;
    }
}
