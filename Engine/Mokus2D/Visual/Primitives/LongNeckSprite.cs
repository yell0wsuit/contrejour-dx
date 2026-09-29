using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Primitives;

public abstract class LongNeckSprite : SpriteBatchNode
{
    public static readonly ISpriteData DefaultSpriteData;

    private bool Created;

    private TintSpriteVertex[] _vertices;

    private short[] _indices;

    private readonly ISpriteData _spriteData;

    private readonly Vector2 _textureLeftTopCoordinate;

    private readonly Vector2 _textureRightBottomCoordinate;

    private readonly Vector2 _textureLeftBottomCoordinate;

    private readonly Vector2 _textureRightTopCoordinate;

    private readonly List<Vector2> _first = new(64);

    private readonly List<Vector2> _second = new(64);

    private readonly List<Vector2> _firstBezier = new(64);

    private readonly List<Vector2> _secondBezier = new(64);

    private readonly List<Vector2> _allPoints = [];

    private readonly List<Pair<Vector2>> _cachedPairs = new(64);

    protected virtual bool HasRecalculateVertices => true;

    protected virtual bool OnScreen => true;

    protected LongNeckSprite(ISpriteData spriteData = null)
    {
        _spriteData = spriteData ?? DefaultSpriteData;
        Texture = _spriteData.Texture;
        Color = Color.Black;
        Rectangle textureRect = _spriteData.TextureRect;
        Vector2 vector = _spriteData.Texture.Bounds.Size();
        _textureLeftTopCoordinate = new Vector2(textureRect.X / vector.X, textureRect.Y / vector.Y);
        _textureRightBottomCoordinate = new Vector2(textureRect.Right / vector.X, textureRect.Bottom / vector.Y);
        _textureLeftBottomCoordinate = new Vector2(_textureLeftTopCoordinate.X, _textureRightBottomCoordinate.Y);
        _textureRightTopCoordinate = new Vector2(_textureRightBottomCoordinate.X, _textureLeftTopCoordinate.Y);
    }

    public abstract void GetPairs(List<Pair<Vector2>> target);

    private void RecalculateVertices()
    {
        _cachedPairs.Clear();
        GetPairs(_cachedPairs);
        if (_cachedPairs.Count <= 2)
        {
            return;
        }
        _first.Clear();
        _second.Clear();
        foreach (Pair<Vector2> cachedPair in _cachedPairs)
        {
            _first.Add(cachedPair.First);
            _second.Add(cachedPair.Second);
        }
        _firstBezier.Clear();
        _secondBezier.Clear();
        AddBezierPointsBezier(_first, _firstBezier);
        AddBezierPointsBezier(_second, _secondBezier);
        CreatePolygonsFirstBezierSecondBezier(_firstBezier, _secondBezier);
    }

    public virtual void AddBezierPointsBezier(List<Vector2> source, List<Vector2> bezier)
    {
        BezierUtil.AddBezierPoints(bezier, source, 6);
    }

    public void CreatePolygonsFirstBezierSecondBezier(List<Vector2> firstBezier, List<Vector2> secondBezier)
    {
        ProcessBezierSecond(firstBezier, secondBezier);
        TintSpriteVertex tintSpriteVertex = new(Vector2.Transform(firstBezier[0], CompositeState.Matrix).ToVector3(), Color, _textureLeftBottomCoordinate, ColorRatio);
        TintSpriteVertex tintSpriteVertex2 = new(Vector2.Transform(secondBezier[0], CompositeState.Matrix).ToVector3(), Color, _textureLeftTopCoordinate, ColorRatio);
        _vertices[0] = tintSpriteVertex;
        _vertices[1] = tintSpriteVertex2;
        int num = 2;
        short num2 = 0;
        short num3 = 0;
        for (int i = 1; i < firstBezier.Count; i++)
        {
            TintSpriteVertex tintSpriteVertex3 = new(Vector2.Transform(firstBezier[i], CompositeState.Matrix).ToVector3(), Color, _textureRightTopCoordinate, ColorRatio);
            TintSpriteVertex tintSpriteVertex4 = new(Vector2.Transform(secondBezier[i], CompositeState.Matrix).ToVector3(), Color, _textureRightBottomCoordinate, ColorRatio);
            _vertices[num] = tintSpriteVertex3;
            _vertices[num + 1] = tintSpriteVertex4;
            num += 2;
            _indices[num2] = num3;
            _indices[num2 + 1] = (short)(num3 + 1);
            _indices[num2 + 2] = (short)(num3 + 3);
            _indices[num2 + 3] = num3;
            _indices[num2 + 4] = (short)(num3 + 3);
            _indices[num2 + 5] = (short)(num3 + 2);
            num2 += 6;
            num3 += 2;
        }
    }

    public void ProcessBezierSecond(List<Vector2> firstBezier, List<Vector2> secondBezier)
    {
        _allPoints.Clear();
        _allPoints.Capacity = firstBezier.Count + secondBezier.Count;
        _allPoints.AddItemsNoGarbage(secondBezier);
        _allPoints.AddItemsNoGarbage(firstBezier, firstBezier.Count - 1, 0);
        TryCreateVectors(_allPoints);
    }

    public void TryCreateVectors(List<Vector2> allPoints)
    {
        if (!Created)
        {
            CreateVectors(allPoints.Count);
            Created = true;
        }
    }

    public virtual void CreateVectors(int allPointsSize)
    {
        _indices = new short[(allPointsSize - 2) * 3];
        _vertices = new TintSpriteVertex[((_indices.Length / 6) + 1) * 2];
    }

    protected override void DrawSprite(VisualState state, Color color)
    {
        if (IsVisibleAndOnScreen)
        {
            if (HasRecalculateVertices || CompositeState.TransformationDirty)
            {
                RecalculateVertices();
            }
            if (_vertices != null)
            {
                Drawer.Draw(_vertices, _indices);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
