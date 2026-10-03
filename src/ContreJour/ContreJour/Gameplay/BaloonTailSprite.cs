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
        private readonly Vertex[] vertices = new Vertex[34];

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
            for (int index = 0; index <= 16; index++)
            {
                float t = index / 16f;
                float inverse = 1f - t;
                Vector2 left = (inverse * inverse * startPair.First) + (2f * inverse * t * middlePair.First) + (t * t * endPair.First);
                Vector2 right = (inverse * inverse * startPair.Second) + (2f * inverse * t * middlePair.Second) + (t * t * endPair.Second);
                vertices[index * 2] = new Vertex(new Vector3(left, 0f), Color.Black, Vector2.Zero);
                vertices[(index * 2) + 1] = new Vertex(new Vector3(right, 0f), Color.Black, Vector2.Zero);
            }
            endSprite.Position = end;
            endSprite.RotationRadians = System.MathF.Atan2(end.Y - middle.Y, end.X - middle.X);
        }

        protected override void DrawPrimitives()
        {
            GraphUtil.DrawTriangleStrip(vertices);
        }
    }
}
