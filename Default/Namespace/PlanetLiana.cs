using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Default.Namespace;

public class PlanetLiana : Node, ILianaDrawData
{
    private readonly List<Vector2> points = [];
    private readonly CosChanger changer;

    private Vector2 middle;

    private readonly float angle;

    public bool Stoped { get; set; }

    public LianaSprite Sprite { get; }

    public PlanetLiana(Vector2 start, Vector2 middle, Vector2 end)
    {
        Box2DConfig defaultConfig = Box2DConfig.DefaultConfig;
        points.Add(defaultConfig.ToVec(start));
        this.middle = defaultConfig.ToVec(middle);
        points.Add(this.middle);
        points.Add(defaultConfig.ToVec(end));
        Sprite = new LianaSprite(this, new Color(50, 50, 50, 255), Maths.Random(2f, 4f));
        AddChild(Sprite);
        angle = Maths.Random(-(float)Math.PI / 6f, (float)Math.PI / 6f);
        changer = new CosChanger(0f - Maths.Random(0.1f, 0.5f), Maths.Random(0.1f, 0.5f), Maths.Random(0.01f, 0.02f));
    }

    public void ReduceRange()
    {
        changer.MinValue /= 2f;
        changer.MaxValue /= 2f;
    }

    public override void Update(float time)
    {
        if (!Stoped)
        {
            changer.Update(time);
            points[1] = middle + VectorUtil.ToVector(changer.Value, angle);
        }
        Sprite.Update(time);
    }

    public override void Draw(VisualState state)
    {
    }

    public Vector2 PositionAt(int index)
    {
        return points[index];
    }

    public int PointsCount()
    {
        return points.Count;
    }
}
