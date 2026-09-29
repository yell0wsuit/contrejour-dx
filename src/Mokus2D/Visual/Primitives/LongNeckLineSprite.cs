using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Primitives;

public abstract class LongNeckLineSprite(ISpriteData spriteData = null) : LongNeckSprite(spriteData)
{
    private readonly List<Vector2> pointsData = [];

    private readonly List<Vector2> controlPoints = [];

    private readonly List<Vector2> bezierLine = [];
    private readonly int bezierPartsCount = 3;

    public float Width { get; set; } = 15f;

    protected abstract void GetPoints(List<Vector2> points);

    protected LongNeckLineSprite(string id)
        : this(Mokus2DGame.LoadSpriteData(id))
    {
    }

    public override void GetPairs(List<Pair<Vector2>> target)
    {
        pointsData.Clear();
        GetPoints(pointsData);
        controlPoints.Clear();
        bezierLine.Clear();
        BezierUtil.AddControlPoints(controlPoints, pointsData);
        BezierUtil.AddBezierPoints(bezierLine, controlPoints, bezierPartsCount);
        Pair<Vector2> orthoPoints;
        for (int i = 0; i < bezierLine.Count - 1; i++)
        {
            orthoPoints = VectorUtil.GetOrthoPoints(bezierLine[i], bezierLine[i], bezierLine[i + 1], Width);
            target.Add(orthoPoints);
        }
        orthoPoints = VectorUtil.GetOrthoPoints(bezierLine.Last(), bezierLine[^2], bezierLine.Last(), Width);
        target.Add(orthoPoints);
    }

    public override void AddBezierPointsBezier(List<Vector2> source, List<Vector2> bezier)
    {
        foreach (Vector2 item in source)
        {
            bezier.Add(item);
        }
    }
}
