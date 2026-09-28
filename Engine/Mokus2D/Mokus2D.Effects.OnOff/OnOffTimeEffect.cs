using System;

using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public abstract class OnOffTimeEffect(Node target, float duration) : OnOffEffect(target)
{
    protected readonly float Duration = duration;

    public Func<bool, float> DurationProvider;

    protected float GetDuration(bool on)
    {
        return DurationProvider != null ? DurationProvider(on) : Duration;
    }
}
