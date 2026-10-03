using System;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    public sealed class BaloonTailSprite : PrimitivesNode
    {
        private readonly BaloonTail tail;
        private readonly LevelBuilderBase builder;
        private readonly Sprite endSprite;
        private const int CurveSteps = 32;
        private const int OutlinePoints = (CurveSteps + 1) * 2;
        private readonly Vertex[] vertices = new Vertex[OutlinePoints];
        private readonly Vector2[] outline = new Vector2[OutlinePoints];
        private readonly Vertex[] stroke = new Vertex[(OutlinePoints + 1) * 4];

        public BaloonTailSprite(BaloonTail tail, LevelBuilderBase builder)
        {
            this.tail = tail;
            this.builder = builder;
            endSprite = new Sprite("newFriend/McBaloonTailEnd");
            AddChild(endSprite);
        }

        public override void Update(float time)
        {
            base.Update(time);
            Visible = tail.Visible;
            Vector2 origin = tail.Start.Position;
            Vector2 middle = builder.ToPoint(tail.Middle.Position - origin);
            Vector2 end = builder.ToPoint(tail.End.Position - origin);
            Pair<Vector2> startPair = ContreDrawUtil.GetPointsPair(Vector2.Zero, Vector2.Zero, middle, 24f);
            Pair<Vector2> middlePair = ContreDrawUtil.GetPointsPair(middle, Vector2.Zero, end, 0f);
            Pair<Vector2> endPair = ContreDrawUtil.GetPointsPair(end, middle, end, 6f);
            for (int index = 0; index <= CurveSteps; index++)
            {
                float t = index / (float)CurveSteps;
                float inverse = 1f - t;
                Vector2 left = (inverse * inverse * startPair.First) + (2f * inverse * t * middlePair.First) + (t * t * endPair.First);
                Vector2 right = (inverse * inverse * startPair.Second) + (2f * inverse * t * middlePair.Second) + (t * t * endPair.Second);
                vertices[index * 2] = new Vertex(new Vector3(left, 0f), Color.Black, Vector2.Zero);
                vertices[(index * 2) + 1] = new Vertex(new Vector3(right, 0f), Color.Black, Vector2.Zero);
                outline[index] = left;
                outline[outline.Length - 1 - index] = right;
            }
            UpdateStroke();
            endSprite.Position = end;
            endSprite.RotationRadians = MathF.Atan2(end.Y - middle.Y, end.X - middle.X);
        }

        protected override void DrawPrimitives()
        {
            GraphUtil.DrawTriangleStrip(stroke);
            GraphUtil.DrawTriangleStrip(vertices);
        }

        private void UpdateStroke()
        {
            // Canvas strokes the closed quadratic outline with width 2 and
            // miter joins before filling it. Sample the same outline here.
            float area = 0f;
            for (int i = 0; i < outline.Length; i++)
            {
                Vector2 a = outline[i];
                Vector2 b = outline[(i + 1) % outline.Length];
                area += (a.X * b.Y) - (b.X * a.Y);
            }
            for (int i = 0; i <= outline.Length; i++)
            {
                int index = i % outline.Length;
                Vector2 point = outline[index];
                Vector2 before = point - outline[(index + outline.Length - 1) % outline.Length];
                Vector2 after = outline[(index + 1) % outline.Length] - point;
                Vector2 n1 = Normal(before, area);
                Vector2 n2 = Normal(after, area);
                Vector2 miter = n1 + n2;
                Vector2 firstOffset = n1;
                Vector2 secondOffset = n2;
                if (miter.LengthSquared() > 0.000001f)
                {
                    miter = Vector2.Normalize(miter);
                    float divisor = Vector2.Dot(miter, n2);
                    // Canvas switches to a bevel when the default miter limit
                    // of ten is exceeded, including a sharply folded tail.
                    if (divisor >= 0.1f)
                    {
                        firstOffset = secondOffset = miter / divisor;
                    }
                }
                stroke[i * 4] = new Vertex(new Vector3(point + firstOffset, 0f), Color.Black, Vector2.Zero);
                stroke[(i * 4) + 1] = new Vertex(new Vector3(point, 0f), Color.Black, Vector2.Zero);
                stroke[(i * 4) + 2] = new Vertex(new Vector3(point + secondOffset, 0f), Color.Black, Vector2.Zero);
                stroke[(i * 4) + 3] = new Vertex(new Vector3(point, 0f), Color.Black, Vector2.Zero);
            }
        }

        private static Vector2 Normal(Vector2 tangent, float area)
        {
            if (tangent.LengthSquared() < 0.000001f)
            {
                return Vector2.Zero;
            }
            tangent = Vector2.Normalize(tangent);
            return area < 0f ? new Vector2(-tangent.Y, tangent.X) : new Vector2(tangent.Y, -tangent.X);
        }
    }
}
