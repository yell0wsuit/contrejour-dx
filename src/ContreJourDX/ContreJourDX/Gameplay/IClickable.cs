using System.Numerics;

using Mokus2D.Input;

namespace ContreJourDX.Gameplay
{
    public interface IClickable
    {
        bool DisableHeroFocus { get; }

        int Priority(Vector2 touchPosition);

        float TouchDistance(Vector2 touchPosition);

        bool AcceptFreeTouches();

        bool UseForZoom();

        bool TouchBegan(Touch touch);

        void TouchEnd(Touch touch);

        bool TouchMove(Touch touch);

        void TouchOut(Touch touch);
    }
}
