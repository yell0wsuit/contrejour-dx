using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class ColorRatioEffect : OnOffTweenEffect<float>
{
	public ColorRatioEffect(Node target, float duration, float onValue = 1f, float offValue = 0f)
		: base(target, duration, NodeValues.ColorRatio, onValue, offValue)
	{
	}
}
