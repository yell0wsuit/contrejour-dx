using System;
using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public abstract class OnOffTimeEffect : OnOffEffect
{
	protected readonly float Duration;

	public Func<bool, float> DurationProvider;

	protected OnOffTimeEffect(Node target, float duration)
		: base(target)
	{
		Duration = duration;
	}

	protected float GetDuration(bool on)
	{
		if (DurationProvider != null)
		{
			return DurationProvider(on);
		}
		return Duration;
	}
}
