using Mokus2D.Util.Data;
using Mokus2D.Visual.Drawing;

namespace Mokus2D.Visual.Primitives;

public class TintSegmentedSprite<T>(string spriteId, ISegmentedSpriteData<T> data) : SegmentedSprite<T>(spriteId, data) where T : struct, ITintVertex
{
    public override Pair<T> GetDefaultPair(float ratio)
    {
        Pair<T> defaultPair = base.GetDefaultPair(ratio);
        defaultPair.First.ColorRatio = CompositeState.ColorRatio;
        defaultPair.Second.ColorRatio = CompositeState.ColorRatio;
        return defaultPair;
    }
}
