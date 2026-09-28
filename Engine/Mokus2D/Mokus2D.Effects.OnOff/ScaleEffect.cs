using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class ScaleEffect : OnOffTweenEffect<Vector2>
{
    public ScaleEffect(Node target, float duration, Vector2 onValue, Vector2 offValue)
        : base(target, duration, NodeValues.ScaleVec, onValue, offValue)
    {
    }
}
