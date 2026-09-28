using System;
using System.Collections.Generic;

using Default.Namespace;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Util;

namespace ContreJourMono.ContreJour.Game.Hero;

public class HeroTail : PrimitivesNode
{
    private const float CENTER_ANGLE = (float)Math.PI / 2f;

    private const float ANGLE_LIMIT = (float)Math.PI * 3f / 8f;

    private const float MIDDLE1_DISTANCE = 30f;

    private const float MIDDLE2_DISTANCE = 50f;

    private const float END_DISTANCE = 70f;

    private const int POINTS = 10;

    public bool LimitAngles;

    public float Speed;

    private float currentAngle;

    private List<PointAndAngle> points;

    private PointAndAngle middle = new PointAndAngle(30f, 0.02f, 0f);

    private PointAndAngle middle2 = new PointAndAngle(50f, 0.019f, (float)Math.PI / 2f);

    private PointAndAngle end = new PointAndAngle(70f, 0.018f, (float)Math.PI);

    private VertexPositionColor[] vertices;

    private List<Vector2> surface = new List<Vector2>(10);

    private readonly List<Vector2> cachedPolygon = new List<Vector2>(64);

    private VertexPositionColor[] border;

    private float borderWidth = 2f;

    public float UpdateSpeed = 1f;

    public float BorderWidth
    {
        get
        {
            return borderWidth;
        }
        set
        {
            borderWidth = value;
        }
    }

    public Color EndColor
    {
        get
        {
            Color color = Color;
            color.A = 0;
            return color;
        }
    }

    public HeroTail(Color color)
    {
        Color = color;
        surface.Resize(10);
        points = new List<PointAndAngle>(new PointAndAngle[3] { middle, middle2, end });
    }

    public HeroTail()
        : this(Color.Black)
    {
    }

    public void SetMovementDirection(float value)
    {
        currentAngle = value - (float)Math.PI;
    }

    public override void Update(float time)
    {
        float angle = currentAngle;
        if (LimitAngles)
        {
            angle = currentAngle.SimplifyAngle(-(float)Math.PI / 2f).Clamp((float)Math.PI / 8f, (float)Math.PI * 7f / 8f);
        }
        PointAndAngle pointAndAngle = points[0];
        float num = angle.SimplifyAngle(pointAndAngle.Angle - (float)Math.PI);
        float num2 = time * 30f * UpdateSpeed;
        foreach (PointAndAngle point in points)
        {
            float num3 = (float)Math.Min((double)Math.Abs(point.Angle + num) / Math.PI * 2.0, 0.5);
            float step = point.AngleStep * Speed * num3 * num2;
            float num4 = (float)Math.PI / 8f;
            point.Angle = point.Angle.StepTo(num, step).Clamp(pointAndAngle.Angle - num4, pointAndAngle.Angle + num4);
            point.Update(Speed / 2f * num2, LimitAngles, num2);
        }
        Triangulate();
    }

    private void Triangulate()
    {
        Pair<Vector2> pointsPair = ContreDrawUtil.GetPointsPair(Vector2.Zero, middle.Position, Vector2.Zero, (25f - borderWidth) * 2f);
        Vector2 center = VectorUtil.Center(middle.Position, middle2.Position);
        Pair<Vector2> pointsPair2 = ContreDrawUtil.GetPointsPair(middle.Position, middle2.Position, center, 12.5f);
        Pair<Vector2> pointsPair3 = ContreDrawUtil.GetPointsPair(center, middle2.Position, center, 12.5f);
        Pair<Vector2> pointsPair4 = ContreDrawUtil.GetPointsPair(middle2.Position, middle2.Position, center, 8.333333f);
        Vector2 item = VectorUtil.Center(pointsPair.First, pointsPair.Second);
        cachedPolygon.Clear();
        cachedPolygon.Add(pointsPair.First);
        cachedPolygon.Add(pointsPair2.First);
        cachedPolygon.Add(pointsPair3.First);
        cachedPolygon.Add(pointsPair4.First);
        cachedPolygon.Add(end.Position);
        cachedPolygon.Add(pointsPair4.Second);
        cachedPolygon.Add(pointsPair3.Second);
        cachedPolygon.Add(pointsPair2.Second);
        cachedPolygon.Add(pointsPair.Second);
        cachedPolygon.Add(item);
        GraphUtil.CreateBezierPoints(cachedPolygon, 2, surface);
        if (vertices == null)
        {
            vertices = new VertexPositionColor[surface.Count];
            border = new VertexPositionColor[surface.Count * 6];
            GraphUtil.CreateGradientBorderColors(border, Color);
        }
        GraphUtil.CreateGradientBorder(surface, borderWidth, border);
        for (int i = 0; i < surface.Count; i++)
        {
            int index = ((i % 2 == 0) ? (i / 2) : (surface.Count - 1 - i / 2));
            ref VertexPositionColor reference = ref vertices[i];
            reference = new VertexPositionColor(surface[index].ToVector3(), Color);
        }
    }

    public override void Draw(VisualState state)
    {
        if (vertices != null)
        {
            base.Draw(state);
        }
    }

    protected override void DrawPrimitives()
    {
        GraphUtil.DrawTriangleStrip(vertices);
        GraphUtil.DrawTriangleList(border);
    }
}
