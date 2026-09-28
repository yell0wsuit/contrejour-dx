using Mokus2D.Visual;

namespace Default.Namespace;

public class CosOpacityChanger(Node target, float minValue, float maxValue, float step) : CosPropertyChanger(target, minValue, maxValue, step)
{
    protected override void SetPropertyValue(float value)
    {
        target.OpacityFloat = value;
    }
}
