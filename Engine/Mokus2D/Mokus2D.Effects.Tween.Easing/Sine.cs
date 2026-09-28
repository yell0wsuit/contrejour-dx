using System;

namespace Mokus2D.Effects.Tween.Easing;

public class Sine
{
	public static readonly Func<float, float> EaseIn = (float k) => EaseInFunction(k);

	public static readonly Func<float, float> EaseInOut = (float k) => EaseInOutFunction(k);

	public static readonly Func<float, float> EaseOut = (float k) => EaseOutFunction(k);

	private static float EaseInFunction(float k)
	{
		return (float)(1.0 - Math.Cos(k * ((float)Math.PI / 2f)));
	}

	private static float EaseInOutFunction(float k)
	{
		return (float)((0.0 - (Math.Cos((float)Math.PI * k) - 1.0)) / 2.0);
	}

	private static float EaseOutFunction(float k)
	{
		return (float)Math.Sin(k * ((float)Math.PI / 2f));
	}
}
