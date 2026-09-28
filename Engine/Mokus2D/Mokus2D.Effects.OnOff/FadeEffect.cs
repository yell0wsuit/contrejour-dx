using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class FadeEffect : OnOffTweenEffect<float>
{
    public FadeEffect(Node target, float onOpacity, float offOpacity, float duration)
        : base(target, duration, NodeValues.OpacityFloat, onOpacity, offOpacity)
    {
    }

    public FadeEffect(Node target, float onOpacity, float duration)
        : this(target, onOpacity, target.OpacityFloat, duration)
    {
    }
}
