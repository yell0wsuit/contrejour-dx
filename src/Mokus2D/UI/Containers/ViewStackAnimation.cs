using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Mokus2D.UI.Containers
{
    public class ViewStackAnimation<T> : AnimationNode where T : ViewSwitcher, new()
    {
        public T ViewSwitcher { get; private set; }

        public Node CurrentView
        {
            get
            {
                T viewStack = ViewSwitcher;
                return viewStack.CurrentView;
            }
            set
            {
                T viewStack = ViewSwitcher;
                viewStack.CurrentView = value;
            }
        }

        public ViewStackAnimation(string name)
            : base(name)
        {
        }

        public ViewStackAnimation(AnimationData animationData)
            : base(animationData)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            RemoveAllChildren();
            ViewSwitcher = new T();
            AddChild(ViewSwitcher);
        }
    }
    public class ViewStackAnimation : ViewStackAnimation<ViewSwitcher>
    {
        public ViewStackAnimation(string name)
            : base(name)
        {
        }

        public ViewStackAnimation(AnimationData animationData)
            : base(animationData)
        {
        }
    }
}
