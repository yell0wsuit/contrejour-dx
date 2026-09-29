using System.Numerics;

using Mokus2D.Input;

namespace ContreJour.Gameplay
{
    public class SuckerEndBodyClip(SuckerBodyClip sucker, object body) : ContreJourBodyClip(sucker.Builder, body, null, null), IClickable
    {
        private Touch touch;

        private readonly SuckerBodyClip sucker = sucker;

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
}
