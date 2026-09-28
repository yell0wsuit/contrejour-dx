using System;

namespace Mokus2D.Effects.Tween.Easing;

public class Quart
{
	public static readonly Func<float, float> EaseIn = (float k) => EaseInFunction(k);

	public static readonly Func<float, float> EaseInOut = (float k) => EaseInOutFunction(k);

	public static readonly Func<float, float> EaseOut = (float k) => EaseOutFunction(k);

	private static float EaseInFunction(float k)
	{
		return k * k * k * k;
	}

	private static float EaseInOutFunction(float k)
	{
		if (!((k *= 2f) < 1f))
		{
			return -0.5f * ((k -= 2f) * k * k * k - 2f);
		}
		return 0.5f * k * k * k * k;
	}

	private static float EaseOutFunction(float k)
	{
		return 0f - ((k -= 1f) * k * k * k - 1f);
	}
}
