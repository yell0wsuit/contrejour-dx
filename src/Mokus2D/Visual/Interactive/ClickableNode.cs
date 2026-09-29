using Mokus2D.Input;

namespace Mokus2D.Visual.Interactive
{
    public abstract class ClickableNode(int priority = 0) : Node, ITouchListener
    {
        private readonly int _priority = priority;

        protected override void OnAddedToStage()
        {
            base.OnAddedToStage();
            Mokus2DGame.Instance.TouchController.AddListener(this, _priority);
        }

        protected override void OnRemovedFromStage()
        {
            base.OnRemovedFromStage();
            Mokus2DGame.Instance.TouchController.RemoveListener(this);
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
    }
}
