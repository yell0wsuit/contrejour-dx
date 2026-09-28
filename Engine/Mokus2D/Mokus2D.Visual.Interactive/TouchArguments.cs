using Mokus2D.Input;

namespace Mokus2D.Visual.Interactive;

public struct TouchArguments(Touch touch, ITouchDispatchNode target)
{
	public readonly Touch Touch = touch;

	public readonly ITouchDispatchNode Target = target;
}
