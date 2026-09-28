using Default.Namespace;

namespace Mokus2D.Util.MathUtils;

public class AsymptoticFloat : IValueProcessor<float>
{
	public float Min;

	public float Max;

	public float Offset;

	public static float GetValue(float value, float min, float max, float asymptoticOffset)
	{
		if (value < min)
		{
			return min - Maths.AsymptoticTo(asymptoticOffset, min - value);
		}
		if (value > max)
		{
			return max + Maths.AsymptoticTo(asymptoticOffset, value - max);
		}
		return value;
	}

	public AsymptoticFloat(float min, float max, float offset)
	{
		Min = min;
		Max = max;
		Offset = offset;
	}

	public float GetValue(float value)
	{
		return GetValue(value, Min, Max, Offset);
	}
}
