using System;

namespace Mokus2D.Effects.Tween.Easing;

public class Back
{
	public static readonly Func<float, float> EaseIn = (float k) => EaseInFunction(k);

	public static readonly Func<float, float> EaseInOut = (float k) => EaseInOutFunction(k);

	public static readonly Func<float, float> EaseOut = (float k) => EaseOutFunction(k);

	public static readonly Func<float, float, float> EaseInWith = (float k, float s) => EaseInFunction(k, s);

	public static readonly Func<float, float, float> EaseInOutWith = (float k, float s) => EaseInOutFunction(k, s);

	public static readonly Func<float, float, float> EaseOutWith = (float k, float s) => EaseOutFunction(k, s);

	private static float EaseInFunction(float k, float s = 1.70158f)
	{
		return k * k * ((s + 1f) * k - s);
	}

	private static float EaseOutFunction(float k, float s = 1.70158f)
	{
		return (k -= 1f) * k * ((s + 1f) * k + s) + 1f;
	}

	private static float EaseInOutFunction(float k, float s = 1.70158f)
	{
		s *= 1.525f;
		if (!((k *= 2f) < 1f))
		{
			return 0.5f * ((k -= 2f) * k * ((s + 1f) * k + s) + 2f);
		}
		return 0.5f * (k * k * ((s + 1f) * k - s));
	}
}
