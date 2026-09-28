using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Mokus2D.UI.Containers;

public class ViewStackAnimation<T> : AnimationNode where T : ViewStack, new()
{
	public T ViewStack { get; private set; }

	public Node CurrentView
	{
		get
		{
			T viewStack = ViewStack;
			return viewStack.CurrentView;
		}
		set
		{
			T viewStack = ViewStack;
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
		ViewStack = new T();
		AddChild(ViewStack);
	}
}
public class ViewStackAnimation : ViewStackAnimation<ViewStack>
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
