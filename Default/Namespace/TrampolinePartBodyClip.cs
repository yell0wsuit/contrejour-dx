using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Input;

namespace Default.Namespace;

public class TrampolinePartBodyClip(LevelBuilderBase builder, object body) : ContreJourBodyClip(builder, body, null, null), IClickable
{
    protected TrampolineBodyClip parent;

    public SnotData Data { get; set; }

    public TrampolineBodyClip Parent
    {
        get => parent;
        set => parent = value;
    }

    public bool DisableHeroFocus => false;

    public bool UseForZoom()
    {
        return false;
    }

    public int Priority(Vector2 touchPosition)
    {
        return 0;
    }

    public bool AcceptFreeTouches()
    {
        return true;
    }

    public bool TouchBegan(Touch touch)
    {
        TrampolineBodyClip trampolineBodyClip = (TrampolineBodyClip)Data.Snot;
        if (!trampolineBodyClip.Dragging)
        {
            trampolineBodyClip.StartDrag(touch);
            return true;
        }
        return false;
    }

    public bool TouchMove(Touch touch)
    {
        return true;
    }

    public void TouchOut(Touch touch)
    {
    }

    public void TouchEnd(Touch touch)
    {
        ((TrampolineBodyClip)Data.Snot).EndDrag();
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
        parent?.OnCollisionStart(body2);
    }
}
