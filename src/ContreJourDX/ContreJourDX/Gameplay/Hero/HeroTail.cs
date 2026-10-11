using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Util;

namespace ContreJourDX.Gameplay.Hero
{
    public class HeroTail : PrimitivesNode
    {
        public bool LimitAngles { get; set; }

        public float Speed { get; set; }

        private float currentAngle;

        private readonly List<PointAndAngle> points;

        private readonly PointAndAngle middle = new(30f, 0.02f, 0f);

        private readonly PointAndAngle middle2 = new(50f, 0.019f, (float)Math.PI / 2f);

        private readonly PointAndAngle end = new(70f, 0.018f, (float)Math.PI);

        private Vertex[] vertices;

        private readonly List<Vector2> surface = new(10);

        private readonly List<Vector2> cachedPolygon = new(64);

        private Vertex[] border;
        public float UpdateSpeed { get; set; } = 1f;

        public float BorderWidth { get; set; } = 2f;

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
            points = [middle, middle2, end];
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
            Pair<Vector2> pointsPair = ContreDrawUtil.GetPointsPair(Vector2.Zero, middle.Position, Vector2.Zero, (25f - BorderWidth) * 2f);
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
                vertices = new Vertex[surface.Count];
                border = new Vertex[surface.Count * 6];
                GraphUtil.CreateGradientBorderColors(border, Color);
            }
            GraphUtil.CreateGradientBorder(surface, BorderWidth, border);
            for (int i = 0; i < surface.Count; i++)
            {
                int index = (i % 2 == 0) ? (i / 2) : (surface.Count - 1 - (i / 2));
                ref Vertex reference = ref vertices[i];
                reference = new Vertex(new Vector3(surface[index], 0f), Color, Vector2.Zero);
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
}
