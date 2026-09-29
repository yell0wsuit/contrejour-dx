namespace Mokus2D.Input
{
    public interface ITouchListener
    {
        bool TouchBegin(Touch touch);

        bool TouchMove(Touch touch);

        void TouchEnd(Touch touch);
    }
}
