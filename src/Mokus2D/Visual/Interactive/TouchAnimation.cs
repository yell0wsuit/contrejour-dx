using Mokus2D.Input;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Interactive
{
    public abstract class TouchAnimation : AnimationNode, ITouchListener
    {
        private readonly TouchListenerDecorator _touchDecorator;

        public bool TouchEnabled
        {
            get => _touchDecorator.Enabled;
            set => _touchDecorator.Enabled = value;
        }

        protected TouchAnimation(string name)
            : base(name)
        {
            _touchDecorator = CreateTouchDecorator();
        }

        protected TouchAnimation(AnimationData animationData)
            : base(animationData)
        {
            _touchDecorator = CreateTouchDecorator();
        }

        private bool IsInteractionsEnabled()
        {
            return Root != null && RootVisible && RootInteractionsEnabled;
        }

        public virtual bool TouchBegin(Touch touch)
        {
            return false;
        }

        public virtual bool TouchMove(Touch touch)
        {
            return false;
        }

        public virtual void TouchEnd(Touch touch)
        {
        }

        private TouchListenerDecorator CreateTouchDecorator()
        {
            TouchListenerDecorator touchListenerDecorator = new(this)
            {
                Filter = IsInteractionsEnabled
            };
            return touchListenerDecorator;
        }
    }
}
