using Default.Namespace;

namespace Mokus2D.Util.MathUtils;

public class AsymptoticFloat(float min, float max, float offset) : IValueProcessor<float>
{
    private float Min = min;

    private float Max = max;

    private float Offset = offset;

    public static float GetValue(float value, float min, float max, float asymptoticOffset)
    {
        return value < min
            ? min - Maths.AsymptoticTo(asymptoticOffset, min - value)
            : value > max ? max + Maths.AsymptoticTo(asymptoticOffset, value - max) : value;
    }

    public float GetValue(float value)
    {
        return GetValue(value, Min, Max, Offset);
    }
}
