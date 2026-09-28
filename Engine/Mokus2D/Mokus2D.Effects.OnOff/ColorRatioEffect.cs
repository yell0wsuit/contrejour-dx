using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class ColorRatioEffect(Node target, float duration, float onValue = 1f, float offValue = 0f) : OnOffTweenEffect<float>(target, duration, NodeValues.ColorRatio, onValue, offValue)
{
}
