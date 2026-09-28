using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RadiusClickListener : ClickListener
{
    protected float clickRadius;

    protected bool disableDrag;

    protected Node target;

    public bool DisableDrag
    {
        get
        {
            return disableDrag;
        }
        set
        {
            disableDrag = value;
        }
    }

    public override bool Enabled
    {
        get
        {
            if (base.Enabled)
            {
                return target.RootVisible;
            }
            return false;
        }
    }

    public RadiusClickListener(Node _target, float _clickRadius, int priority = 0)
        : base(priority)
    {
        clickRadius = _clickRadius;
        target = _target;
    }

    private bool SpriteContainsPoint(Touch touch)
    {
        if (target.Root == null)
        {
            return false;
        }
        return target.GlobalToLocal(touch.Position).Length() < clickRadius;
    }

    public override bool TouchBegin(Touch touch)
    {
        if (SpriteContainsPoint(touch))
        {
            base.TouchBegin(touch);
            return true;
        }
        return false;
    }

    protected override bool IsOutStartPosition(Touch touch, Vector2 _startPosition)
    {
        bool flag = !SpriteContainsPoint(touch);
        if (disableDrag)
        {
            flag |= base.IsOutStartPosition(touch, _startPosition);
        }
        return flag;
    }
}
