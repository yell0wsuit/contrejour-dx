using System.Numerics;

using FarseerPhysics.Dynamics;

using Mokus2D.Input;

namespace ContreJourDX.Gameplay
{
    public class SnotEye(SnotBodyClip snot, Body body) : ContreJourDXBodyClip(snot.Builder, body, null, null), IClickable
    {
        protected bool HasRelease { get; set; }

        public SnotBodyClip Snot { get; } = snot;

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
            return !Snot.Joined ? -10 : 1;
        }

        public virtual bool TouchBegan(Touch touch)
        {
            HasRelease = true;
            return true;
        }

        public virtual void TouchEnd(Touch touch)
        {
            CheckTouchDistance(touch);
            if (HasRelease)
            {
                Snot.ReleaseSnot();
                ContreJourDXGame.FocusOnHero();
                HasRelease = false;
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
            CheckTouchDistance(touch, Vector2.Distance(Builder.TouchRootVec(touch), Body.Position));
        }

        protected virtual void CheckTouchDistance(Touch touch, float distance)
        {
            if (distance > 1.8333334f)
            {
                HasRelease = false;
                FreeTouch(touch);
            }
        }

        protected virtual void FreeTouch(Touch touch)
        {
            Game.FreeTouch(touch);
        }
    }
}
