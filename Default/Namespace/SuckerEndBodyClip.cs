using Microsoft.Xna.Framework;

using Mokus2D.Input;

namespace Default.Namespace;

public class SuckerEndBodyClip(SuckerBodyClip _sucker, object body) : ContreJourBodyClip(_sucker.Builder, body, null, null), IClickable
{
    private Touch touch;

    private SuckerBodyClip sucker = _sucker;

    public bool DisableHeroFocus => true;

    public int Priority(Vector2 touchPosition)
    {
        return 1;
    }

    public bool AcceptFreeTouches()
    {
        return false;
    }

    public bool UseForZoom()
    {
        return false;
    }

    public bool TouchBegan(Touch touch)
    {
        if (this.touch == null && !sucker.Dragging)
        {
            this.touch = touch;
            sucker.StartDrag(this.touch);
            return true;
        }
        return false;
    }

    public void TouchEnd(Touch touch)
    {
        sucker.FinishDrag();
        this.touch = null;
    }

    public bool TouchMove(Touch touch)
    {
        return true;
    }

    public void TouchOut(Touch touch)
    {
    }
}
