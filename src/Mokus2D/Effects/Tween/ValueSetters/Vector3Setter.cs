using System.Numerics;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Effects.Tween.ValueSetters
{
    public class Vector3Setter : ValueSetter<Vector3>
    {
        protected override Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            return XnaMath.Lerp(value1, value2, amount);
        }
    }
}
