using System;
using System.Collections.Generic;

using ContreJour.Clips.common;
using ContreJour.Primitives;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Default.Namespace;

public class PlanetSnot : LongNeckSprite, IDepthDependent
{
    private const float RADIUS = 40f;

    private const float MIDDLE_WIDTH = 5f;

    private const float END_WIDTH = 20f;

    private const float START_WIDTH = 10f;

    private readonly PlanetSnotEye _eye;

    public readonly Sprite BaseSprite;

    protected Vector2 middle;

    protected Vector2 end;

    protected Vector2 targetEnd;

    protected ushort opacity;

    protected float depth;

    protected Vector2 endInit;

    protected Vector2 middleInit;

    private static readonly Vector2 END = new(0f, 80f);

    private static readonly Vector2 MIDDLE = new(0f, 30f);

    public float Depth
    {
        get => depth;
        set => depth = value;
    }

    public PlanetSnot(PlanetSnotEye eye)
    {
        endInit = END;
        middleInit = MIDDLE;
        BaseSprite = new McFlowerHead();
        _eye = eye;
        _eye.Position = end;
        middle = middleInit;
        end = Vector2.Zero;
        borderWidth = 4f;
        opacity = 255;
    }

    public override void GetPairs(List<Pair<Vector2>> target)
    {
        target.Add(ContreDrawUtil.GetPointsPair(Vector2.Zero, Vector2.Zero, middle, 10f));
        target.Add(ContreDrawUtil.GetPointsPair(middle, Vector2.Zero, middle, 5f));
        target.Add(ContreDrawUtil.GetPointsPair(end, middle, end, 20f));
    }

    public override void Update(float time)
    {
        if (Maths.FuzzyEquals(depth, 1f))
        {
            Vector2 vector = VectorUtil.ToVector(_eye.ViewDistance * 40f, _eye.ViewAngle);
            targetEnd = endInit + vector;
            float num = end.DistanceTo(targetEnd);
            middle = VectorUtil.StepTo(middle, middleInit, 1f);
            end = VectorUtil.StepTo(end, targetEnd, Math.Min(1f, num / 5f));
        }
        else if (depth > 0.6f)
        {
            targetEnd = new Vector2(-10f, -10f);
            middle = VectorUtil.StepTo(middle, new Vector2(-5f, -5f), 10f);
            end = VectorUtil.StepTo(end, targetEnd, 10f);
        }
        else
        {
            middle = new Vector2(-5f, -5f);
            targetEnd = new Vector2(-10f, -10f);
            end = targetEnd;
        }
        _eye.Position = end;
        BaseSprite.Position = end;
        base.Update(time);
    }

    public override void Draw(VisualState state)
    {
        base.Draw(state);
    }
}
