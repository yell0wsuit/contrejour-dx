using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class SnotEye : ContreJourBodyClip, IClickable
{
    private const float MAX_RADIUS = 1.8333334f;

    protected bool hasRelease;

    protected SnotBodyClip snot;

    public SnotBodyClip Snot => snot;

    public bool DisableHeroFocus => true;

    public SnotEye(SnotBodyClip _snot, Body _body)
        : base(_snot.Builder, _body, null, null)
    {
        snot = _snot;
    }

    public bool AcceptFreeTouches()
    {
        return false;
    }

    public bool UseForZoom()
    {
        return false;
    }

    public virtual int Priority(Vector2 touchPoint)
    {
        if (!snot.Joined)
        {
            return -10;
        }
        return 1;
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
            ((ContreJourGame)builder.Game).FocusOnHero();
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
        base.Game.FreeTouch(touch);
    }
}
