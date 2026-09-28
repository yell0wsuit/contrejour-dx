using Mokus2D.Visual;

namespace Default.Namespace;

public class CosRotationChanger : CosPropertyChanger
{
    public CosRotationChanger(Node target, float maxValue, float step)
        : base(target, 0f - maxValue, maxValue, step)
    {
    }

    protected override void SetPropertyValue(float value)
    {
        target.RotationDegrees = value;
    }
}
