using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Primitives
{
    public class BezierSegmentedData<T> : ISegmentedSpriteData<T>, IUpdatable where T : struct, IVertex
    {
        private readonly ISegmentedSpriteData<T> _originalData;

        private readonly int _bezierSegmentsCount;

        private readonly List<Pair<T>> _originalLines = [];

        private readonly List<Vector2> _firstBezierPoints = [];

        private readonly List<Vector2> _secondBezierPoints = [];

        private int _spriteSegmentsCount;

        public int PairsCount { get; private set; }

        public bool IsDirty
        {
            get
            {
                if (_originalData.IsDirty)
                {
                    RefreshPairsCount();
                }
                return _originalData.IsDirty;
            }
        }

        public BezierSegmentedData(ISegmentedSpriteData<T> originalData, int bezierSegmentsCount)
        {
            _originalData = originalData;
            _bezierSegmentsCount = bezierSegmentsCount;
            RefreshPairsCount();
        }

        private void RefreshPairsCount()
        {
            _spriteSegmentsCount = _originalData.PairsCount - 1;
            int num = _bezierSegmentsCount * (_spriteSegmentsCount - 1);
            PairsCount = num <= 0 ? _originalData.PairsCount : num + 3;
        }

        public void FillLines(SegmentedSprite<T> sprite, List<Pair<T>> lines, ref Matrix matrix)
        {
            _originalLines.Clear();
            _originalData.FillLines(sprite, _originalLines, ref matrix);
            if (_originalLines.Count < 3)
            {
                lines.AddRange(_originalLines);
            }
            else
            {
                Interpolate(lines);
            }
        }

        private void Interpolate(List<Pair<T>> lines)
        {
            lines.Add(_originalLines[0]);
            Pair<T> pair = LerpVertices(_originalLines[0], _originalLines[1], 0.5f);
            for (int i = 0; i < _originalLines.Count - 2; i++)
            {
                Pair<T> start = pair;
                Pair<T> start2 = _originalLines[i + 1];
                Pair<T> end = _originalLines[i + 2];
                pair = LerpVertices(start2, end, 0.5f);
                _firstBezierPoints.Clear();
                _secondBezierPoints.Clear();
                BezierUtil.GetBezierPoints(start.First.Position.ToVector2(), start2.First.Position.ToVector2(), pair.First.Position.ToVector2(), _bezierSegmentsCount, insertLast: false, _firstBezierPoints);
                BezierUtil.GetBezierPoints(start.Second.Position.ToVector2(), start2.Second.Position.ToVector2(), pair.Second.Position.ToVector2(), _bezierSegmentsCount, insertLast: false, _secondBezierPoints);
                for (int j = 0; j < _bezierSegmentsCount; j++)
                {
                    Pair<T> item = LerpVertices(start, pair, j / (float)_bezierSegmentsCount);
                    item.First.Position = _firstBezierPoints[j].ToVector3();
                    item.Second.Position = _secondBezierPoints[j].ToVector3();
                    lines.Add(item);
                }
            }
            _firstBezierPoints.Clear();
            _secondBezierPoints.Clear();
            lines.Add(pair);
            lines.Add(_originalLines[^1]);
        }

        protected virtual Pair<T> LerpVertices(Pair<T> value1, Pair<T> value2, float amount)
        {
            return new Pair<T>
            {
                First =
                {
                    Position = Vector3.Lerp(value1.First.Position, value2.First.Position, amount),
                    Color = value1.First.Color.LerpTo(value2.First.Color, amount),
                    TextureCoordinate = value1.First.TextureCoordinate.LerpTo(value2.First.TextureCoordinate, amount)
                },
                Second =
                {
                    Position = Vector3.Lerp(value1.Second.Position, value2.Second.Position, amount),
                    Color = value1.Second.Color.LerpTo(value2.Second.Color, amount),
                    TextureCoordinate = value1.Second.TextureCoordinate.LerpTo(value2.Second.TextureCoordinate, amount)
                }
            };
        }

        public void Update(float time)
        {
            _originalData.Update(time);
        }
    }
}
