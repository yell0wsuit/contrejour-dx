using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class SnotEye(SnotBodyClip _snot, Body body) : ContreJourBodyClip(_snot.Builder, body, null, null), IClickable
{
    protected bool hasRelease;

    protected SnotBodyClip snot = _snot;

    public SnotBodyClip Snot => snot;

    public bool DisableHeroFocus => true;

    public bool AcceptFreeTouches()
    {
        return false;
    }

    public bool UseForZoom()
    {
        return false;
    }

    public virtual int Priority(Vector2 touchPosition)
    {
        return !snot.Joined ? -10 : 1;
    }

    public virtual bool TouchBegan(Touch touch)
    {
        hasRelease = true;
        return true;
    }

    public virtual void TouchEnd(Touch touch)
    {
        CheckTouchDistance(touch);
        if (hasRelease)
        {
            snot.ReleaseSnot();
            ContreJourGame.FocusOnHero();
            hasRelease = false;
        }
    }

    public virtual bool TouchMove(Touch touch)
    {
        CheckTouchDistance(touch);
        return true;
    }

    public void TouchOut(Touch touch)
    {
    }

    private void CheckTouchDistance(Touch touch)
    {
        CheckTouchDistance(touch, builder.TouchRootVec(touch).DistanceTo(Body.Position));
    }

    protected virtual void CheckTouchDistance(Touch touch, float distance)
    {
        if (distance > 1.8333334f)
        {
            hasRelease = false;
            FreeTouch(touch);
        }
    }

    protected virtual void FreeTouch(Touch touch)
    {
        Game.FreeTouch(touch);
    }
}
