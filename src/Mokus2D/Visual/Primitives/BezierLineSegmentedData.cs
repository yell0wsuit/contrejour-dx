using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Primitives;

public class BezierLineSegmentedData<T>(float width, int bezierSegmentsCount) : LineSegmentedData<T>(width), ISegmentedSpriteData<T>, IUpdatable where T : struct, IVertex
{
    private readonly int _bezierSegmentsCount = bezierSegmentsCount;

    private readonly List<Vector2> _bezierLine = [];

    public override int PairsCount => BezierUtil.GetBezierLinePointsCount(Line.Count, _bezierSegmentsCount);

    public override void FillLines(SegmentedSprite<T> sprite, List<Pair<T>> lines, ref Matrix matrix)
    {
        FillBezierLine();
        FillLines(sprite, lines, ref matrix, _bezierLine);
    }

    private void FillBezierLine()
    {
        _bezierLine.Clear();
        BezierUtil.CreateBezierLine(Line, _bezierLine, _bezierSegmentsCount);
    }
}
