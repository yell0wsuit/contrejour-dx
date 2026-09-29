using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class CosRotationChanger(Node target, float maxValue, float step) : CosPropertyChanger(target, 0f - maxValue, maxValue, step)
    {
        protected override void SetPropertyValue(float value)
        {
            Target.RotationDegrees = value;
        }
    }
}
