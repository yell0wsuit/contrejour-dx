using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mokus2D.Input;

namespace ContreJourDX.Gameplay
{
    public class TrampolinePartBodyClip(LevelBuilderBase builder, object body) : ContreJourDXBodyClip(builder, body, null, null), IClickable
    {
        public SnotData Data { get; set; }

        public TrampolineBodyClip Parent { get; set; }

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
            Parent?.OnCollisionStart(body2);
        }
    }
}
