using System.Collections.Generic;

using ContreJour.Primitives;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;

namespace Default.Namespace;

public class SnotSprite : LongNeckSprite
{
    public const int CIRCLE_SEGMENTS = 12;

    protected SnotBodyClipBase snot;

    protected SnotData data;

    protected float startWidth;

    protected float endWidth;

    protected float startWidthPixels;

    protected float endWidthPixels;

    protected float centerWidth;

    protected List<Vector2> surface;

    public SnotSprite(SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth)
    {
        snot = _snot;
        data = snot.Physics;
        startWidth = _startWidth;
        endWidth = _endWidth;
        startWidthPixels = startWidth / (1f / 30f);
        endWidthPixels = endWidth / (1f / 30f);
        centerWidth = _centerWidth;
    }

    public override void GetPairs(List<Pair<Vector2>> target)
    {
        Vector2 startPosition = snot.StartPosition;
        target.Add(ContreDrawUtil.ccp2Pair(ContreDrawUtil.GetPointsPairStartEndWidthResult(startPosition, startPosition, data.BodyAt(0).Position, startWidth)));
        Vector2 start = startPosition;
        Body val = null;
        for (int i = 0; i < data.BodiesSize() - 1; i++)
        {
            val = data.BodyAt(i);
            Body val2 = data.BodyAt(i + 1);
            target.Add(ContreDrawUtil.ccp2Pair(ContreDrawUtil.GetPointsPairStartEndWidthResult(val.Position, start, val2.Position, centerWidth)));
            start = val2.Position;
            if (i < data.BodiesSize() - 2)
            {
                target.Add(ContreDrawUtil.ccp2Pair(ContreDrawUtil.GetPointsPairStartEndWidthResult((val.Position + val2.Position) * 0.5f, val.Position, val2.Position, centerWidth)));
            }
        }
        target.Add(ContreDrawUtil.ccp2Pair(ContreDrawUtil.GetPointsPairStartEndWidthResult(snot.EndPosition(), val.Position, snot.EndPosition(), endWidth)));
    }
}
