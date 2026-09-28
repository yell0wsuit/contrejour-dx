using System;
using System.Collections.Generic;

using ContreJour.Primitives;

using Microsoft.Xna.Framework;

using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class LianaSprite : LongNeckSprite
{
    protected float width;

    protected ILianaDrawData data;

    protected List<int> partsLength = [];

    private float MAX_PART_LENGHT = 4f / 15f;

    public LianaSprite(ILianaDrawData _data, Color neckColor, float _width, float _borderWidth)
    {
        data = _data;
        width = _width * (1f / 30f);
        base.Color = neckColor;
        CreateParts();
        Update(0f);
    }

    public LianaSprite(ILianaDrawData _data, Color _color)
        : this(_data, _color, 0f, Maths.Random(8f, 10f))
    {
    }

    public void CreateParts()
    {
        partsLength.Add(2);
        for (int i = 0; i < data.PointsCount() - 2; i++)
        {
            Vector2 source = data.PositionAt(i);
            Vector2 vector = data.PositionAt(i + 1);
            Vector2 target = data.PositionAt(i + 2);
            int num = (int)Math.Ceiling(((source.DistanceTo(vector) / 2f) + (vector.DistanceTo(target) / 2f)) / MAX_PART_LENGHT);
            num += num % 2;
            partsLength.Add(num);
        }
        partsLength.Add(2);
    }

    public override void GetPairs(List<Pair<Vector2>> target)
    {
        Vector2 vector = data.PositionAt(0);
        Pair<Vector2> pointsPairStartEndWidthResult = ContreDrawUtil.GetPointsPairStartEndWidthResult(vector, vector, data.PositionAt(1), width);
        target.Add(pointsPairStartEndWidthResult);
        target.Add(pointsPairStartEndWidthResult);
        for (int i = 1; i < data.PointsCount(); i++)
        {
            Vector2 start = data.PositionAt(i - 1);
            Vector2 vector2 = data.PositionAt(i);
            vector = VectorUtil.Center(start, vector2);
            target.Add(ContreDrawUtil.GetPointsPairStartEndWidthResult(vector, vector, vector2, width));
            target.Add(ContreDrawUtil.GetPointsPairStartEndWidthResult(vector2, start, data.PositionAt(Math.Min(i + 1, data.PointsCount() - 1)), width));
        }
        Vector2 vector3 = data.PositionAt(data.PointsCount() - 1);
        pointsPairStartEndWidthResult = ContreDrawUtil.GetPointsPairStartEndWidthResult(vector3, vector, vector3, width);
        target.Add(pointsPairStartEndWidthResult);
        target.Add(pointsPairStartEndWidthResult);
    }

    public override void AddBezierPointsBezier(List<Vector2> source, List<Vector2> bezier)
    {
        BezierUtil.AddBezierPoints(bezier, source, partsLength);
    }
}
