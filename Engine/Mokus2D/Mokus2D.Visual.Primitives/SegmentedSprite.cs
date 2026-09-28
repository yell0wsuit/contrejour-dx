using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Animation;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Primitives;

public class SegmentedSprite<T> : SpriteBatchNode where T : struct, IVertex
{
    private readonly ISegmentedSpriteData<T> Data;

    private T[] _vertices;

    private short[] _indices;

    private int _currentSegmentsCount;

    private bool _firstDraw = true;

    private readonly List<Pair<T>> _lines = [];

    private readonly Vector2 _leftTop;

    private readonly Vector2 _leftBottom;

    private readonly Vector2 _rightTop;

    private readonly Vector2 _rightBottom;

    private Color _color;

    public SegmentedSprite(string spriteId, ISegmentedSpriteData<T> data)
        : this(Mokus2DGame.LoadSpriteData(spriteId), data)
    {
    }

    public SegmentedSprite(ISpriteData spriteData, ISegmentedSpriteData<T> data)
        : base(spriteData.Texture)
    {
        Data = data;
        _currentSegmentsCount = GetSegmentsCount();
        _vertices = new T[GetVerticesCount()];
        _indices = new short[GetIndicesCount()];
        GraphUtil.FillSegmentsIndices(_indices, _indices.Length);
        _leftTop = Texture.GetTextureCoords(spriteData.TextureRect.LeftTop());
        _leftBottom = Texture.GetTextureCoords(spriteData.TextureRect.LeftBottom());
        _rightTop = Texture.GetTextureCoords(spriteData.TextureRect.RightTop());
        _rightBottom = Texture.GetTextureCoords(spriteData.TextureRect.RightBottom());
    }

    private int GetIndicesCount()
    {
        return _currentSegmentsCount * 6;
    }

    private int GetVerticesCount()
    {
        return (_currentSegmentsCount * 2) + 2;
    }

    private int GetSegmentsCount()
    {
        return Math.Max(Data.PairsCount - 1, 0);
    }

    public override void Update(float time)
    {
        base.Update(time);
        Data.Update(time);
    }

    public virtual Pair<T> GetDefaultPair(float ratio)
    {
        return new Pair<T>(new T
        {
            Color = _color,
            TextureCoordinate = _leftTop.LerpTo(_rightTop, ratio)
        }, new T
        {
            Color = _color,
            TextureCoordinate = _leftBottom.LerpTo(_rightBottom, ratio)
        });
    }

    protected override void DrawSprite(VisualState state, Color color)
    {
        if (Data.IsDirty || CompositeState.TransformationDirty || _firstDraw)
        {
            _lines.Clear();
            int segmentsCount = GetSegmentsCount();
            if (_currentSegmentsCount != segmentsCount)
            {
                ResizeArrays(segmentsCount);
            }
            _color = color;
            Data.FillLines(this, _lines, ref CompositeState.Matrix);
            for (int i = 0; i < _lines.Count; i++)
            {
                _vertices[i * 2] = _lines[i].First;
                _vertices[(i * 2) + 1] = _lines[i].Second;
            }
            _firstDraw = false;
        }
        Drawer.Draw(_vertices, _indices);
    }

    private void ResizeArrays(int segmentsCount)
    {
        _currentSegmentsCount = segmentsCount;
        Array.Resize(ref _vertices, GetVerticesCount());
        Array.Resize(ref _indices, GetIndicesCount());
        GraphUtil.FillSegmentsIndices(_indices, _indices.Length);
    }
}
