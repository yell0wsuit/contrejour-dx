using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Primitives
{
    public class LineSegmentedData<T>(float width) : ISegmentedSpriteData<T>, IUpdatable where T : struct, IVertex
    {
        protected List<Vector2> Line { get; } = [];

        private readonly float _width = width;

        public virtual int PairsCount => Line.Count;

        public bool IsDirty { get; private set; }

        public Vector2 this[int i]
        {
            get => Line[i];
            set
            {
                if (Line[i] != value)
                {
                    Line[i] = value;
                    IsDirty = true;
                }
            }
        }

        public int Count => Line.Count;

        public void Add(Vector2 position)
        {
            Line.Add(position);
            IsDirty = true;
        }

        public void Update(float time)
        {
        }

        public virtual void FillLines(SegmentedSprite<T> sprite, List<Pair<T>> lines, ref Matrix matrix)
        {
            FillLines(sprite, lines, ref matrix, Line);
        }

        protected void FillLines(SegmentedSprite<T> sprite, List<Pair<T>> lines, ref Matrix matrix, List<Vector2> line)
        {
            if (line.Count > 1)
            {
                AddOrthoPoints(sprite, lines, 0, line[0], line[0], line[1], ref matrix);
                for (int i = 1; i < line.Count - 1; i++)
                {
                    AddOrthoPoints(sprite, lines, i, line[i], line[i - 1], line[i + 1], ref matrix);
                }
                AddOrthoPoints(sprite, lines, line.Count - 1, line[^1], line[^2], line[^1], ref matrix);
            }
        }

        private void AddOrthoPoints(SegmentedSprite<T> sprite, List<Pair<T>> lines, int index, Vector2 center, Vector2 start, Vector2 end, ref Matrix matrix)
        {
            Vector2 vector = end - start;
            if (vector == Vector2.Zero)
            {
                vector = Vector2.One;
            }
            Pair<Vector2> orthoPoints = VectorUtil.GetOrthoPoints(center, vector, _width);
            Pair<T> defaultPair = sprite.GetDefaultPair(index / (float)(PairsCount - 1));
            defaultPair.First.Position = new Vector3(Vector2.Transform(orthoPoints.First, matrix), 0f);
            defaultPair.Second.Position = new Vector3(Vector2.Transform(orthoPoints.Second, matrix), 0f);
            lines.Add(defaultPair);
        }

        public void Clear()
        {
            IsDirty = true;
            Line.Clear();
        }
    }
}
