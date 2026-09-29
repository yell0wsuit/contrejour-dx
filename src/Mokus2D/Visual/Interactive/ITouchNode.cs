using Mokus2D.Input;

namespace Mokus2D.Visual.Interactive
{
    public interface ITouchNode
    {
        bool Clickable { get; }

        bool TouchBegin(Touch touch);

        bool TouchMove(Touch touch);

        bool TouchOut(Touch touch);

        void TouchEnd(Touch touch);
    }
}
