using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Drawing;

namespace Mokus2D.Visual.Primitives;

public class TintBezierSegmentedData<T>(ISegmentedSpriteData<T> originalData, int bezierSegmentsCount) : BezierSegmentedData<T>(originalData, bezierSegmentsCount) where T : struct, ITintVertex
{
    protected override Pair<T> LerpVertices(Pair<T> value1, Pair<T> value2, float amount)
    {
        Pair<T> result = base.LerpVertices(value1, value2, amount);
        result.First.ColorRatio = amount.Lerp(value1.First.ColorRatio, value2.First.ColorRatio);
        result.Second.ColorRatio = amount.Lerp(value1.Second.ColorRatio, value2.Second.ColorRatio);
        return result;
    }
}
