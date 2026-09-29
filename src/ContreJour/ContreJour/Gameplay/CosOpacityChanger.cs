using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class CosOpacityChanger(Node target, float minValue, float maxValue, float step) : CosPropertyChanger(target, minValue, maxValue, step)
{
    protected override void SetPropertyValue(float value)
    {
        Target.OpacityFloat = value;
    }
}
