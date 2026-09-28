using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Interactive;

public class ClickableAnimation : AnimationNode
{
	public ClickableAnimation(string name)
		: base(name)
	{
	}

	public ClickableAnimation(AnimationData animationData)
		: base(animationData)
	{
	}
}
