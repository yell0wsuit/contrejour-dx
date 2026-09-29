using System.Numerics;

namespace Mokus2D.Effects.Tween.ValueSetters
{
    public class Vector3Setter : ValueSetter<Vector3>
    {
        protected override Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            return Vector3.Lerp(value1, value2, amount);
        }
    }
}
