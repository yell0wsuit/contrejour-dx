using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Drawing;

namespace Mokus2D.Visual.Primitives;

public class TintBezierSegmentedData<T> : BezierSegmentedData<T> where T : struct, ITintVertex
{
	public TintBezierSegmentedData(ISegmentedSpriteData<T> originalData, int bezierSegmentsCount)
		: base(originalData, bezierSegmentsCount)
	{
	}

	protected override Pair<T> LerpVertices(Pair<T> start, Pair<T> end, float amount)
	{
		Pair<T> result = base.LerpVertices(start, end, amount);
		result.First.ColorRatio = amount.Lerp(start.First.ColorRatio, end.First.ColorRatio);
		result.Second.ColorRatio = amount.Lerp(start.Second.ColorRatio, end.Second.ColorRatio);
		return result;
	}
}
