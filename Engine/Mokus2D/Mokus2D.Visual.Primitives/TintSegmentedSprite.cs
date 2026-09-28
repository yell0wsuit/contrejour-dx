using Mokus2D.Util.Data;
using Mokus2D.Visual.Drawing;

namespace Mokus2D.Visual.Primitives;

public class TintSegmentedSprite<T> : SegmentedSprite<T> where T : struct, ITintVertex
{
	public TintSegmentedSprite(string spriteId, ISegmentedSpriteData<T> data)
		: base(spriteId, data)
	{
	}

	public override Pair<T> GetDefaultPair(float ratio)
	{
		Pair<T> defaultPair = base.GetDefaultPair(ratio);
		defaultPair.First.ColorRatio = base.CompositeState.ColorRatio;
		defaultPair.Second.ColorRatio = base.CompositeState.ColorRatio;
		return defaultPair;
	}
}
