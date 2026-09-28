using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Primitives;

public abstract class LongNeckLineSprite : LongNeckSprite
{
    private readonly List<Vector2> pointsData = new List<Vector2>();

    private readonly List<Vector2> controlPoints = new List<Vector2>();

    private readonly List<Vector2> bezierLine = new List<Vector2>();

    private float _width = 15f;

    protected int bezierPartsCount = 3;

    public float Width
    {
        get
        {
            return _width;
        }
        set
        {
            _width = value;
        }
    }

    protected abstract void GetPoints(List<Vector2> points);

    protected LongNeckLineSprite(string id)
        : this(Mokus2DGame.LoadSpriteData(id))
    {
    }

    protected LongNeckLineSprite(ISpriteData spriteData = null)
        : base(spriteData)
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
            orthoPoints = VectorUtil.GetOrthoPoints(bezierLine[i], bezierLine[i], bezierLine[i + 1], _width);
            target.Add(orthoPoints);
        }
        orthoPoints = VectorUtil.GetOrthoPoints(bezierLine.Last(), bezierLine[bezierLine.Count - 2], bezierLine.Last(), _width);
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
