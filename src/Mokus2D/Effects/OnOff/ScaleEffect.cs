using System.Numerics;

using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff
{
    public class ScaleEffect(Node target, float duration, Vector2 onValue, Vector2 offValue) : OnOffTweenEffect<Vector2>(target, duration, NodeValues.ScaleVec, onValue, offValue)
    {
    }
}
