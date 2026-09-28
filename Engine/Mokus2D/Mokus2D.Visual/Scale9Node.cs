using System.Collections.Generic;

using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual;

public class Scale9Node : Node, ISizeNode, IDataReloadable
{
    protected readonly Sprite LeftTop;

    protected readonly Sprite Top;

    protected readonly Sprite RightTop;

    protected readonly Sprite Left;

    protected readonly Sprite Middle;

    protected readonly Sprite Right;

    protected readonly Sprite LeftBottom;

    protected readonly Sprite Bottom;

    protected readonly Sprite RightBottom;

    private readonly float _leftSize;

    private readonly float _rightSize;

    private readonly float _topSize;

    private readonly float _bottomSize;

    private Vector2 _size;

    private float _overlap;

    private readonly Flag _anchorsDirty = new(on: false);

    private readonly Dictionary<Sprite, Vector2> _childrenAnchors = [];

    public float Overlap
    {
        get => _overlap;
        set
        {
            _overlap = value;
            RefreshSize();
        }
    }

    public float Width
    {
        get => Size.X;
        set => Size = new Vector2(value, Size.Y);
    }

    public float Height
    {
        get => Size.Y;
        set => Size = new Vector2(Size.X, value);
    }

    public Vector2 Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                RefreshSize();
            }
        }
    }

    public static Scale9Node CreateHorizontal(Sprite left, Sprite middle, Sprite right)
    {
        return new Scale9Node(middle, null, null, null, left, right);
    }

    public static Scale9Node CreateVertical(Sprite top, Sprite middle, Sprite bottom)
    {
        return new Scale9Node(middle, null, top, null, null, null, null, bottom);
    }

    public Scale9Node(Sprite middle, Sprite leftTop = null, Sprite top = null, Sprite rightTop = null, Sprite left = null, Sprite right = null, Sprite leftBottom = null, Sprite bottom = null, Sprite rightBottom = null)
    {
        LeftTop = TryAddChild(leftTop, new Vector2(1f));
        Top = TryAddChild(top, new Vector2(0.5f, 1f));
        RightTop = TryAddChild(rightTop, new Vector2(0f, 1f));
        Left = TryAddChild(left, new Vector2(1f, 0.5f));
        Right = TryAddChild(right, new Vector2(0f, 0.5f));
        LeftBottom = TryAddChild(leftBottom, new Vector2(1f, 0f));
        Bottom = TryAddChild(bottom, new Vector2(0.5f, 0f));
        RightBottom = TryAddChild(rightBottom, new Vector2(0f));
        Middle = TryAddChild(middle, new Vector2(0.5f));
        _leftSize = MaxWidth(LeftTop, Left, LeftBottom);
        _rightSize = MaxWidth(RightTop, Right, RightBottom);
        _topSize = MaxHeight(LeftTop, Top, RightTop);
        _bottomSize = MaxWidth(LeftBottom, Bottom, RightBottom);
        _size = new Vector2(Middle.Size.X + _leftSize + _rightSize, Middle.Size.Y + _topSize + _bottomSize);
        RefreshPositions(Middle.Size);
    }

    private void RefreshAnchors()
    {
        foreach (KeyValuePair<Sprite, Vector2> childrenAnchor in _childrenAnchors)
        {
            childrenAnchor.Key.Anchor = childrenAnchor.Value;
        }
    }

    protected virtual void RefreshSize()
    {
        Vector2 middleSize = RefreshMiddleScale();
        RefreshBorderScales(middleSize);
        RefreshPositions(middleSize);
    }

    private Vector2 RefreshMiddleScale()
    {
        Vector2 vector = new(_size.X - _leftSize - _rightSize, _size.Y - _topSize - _bottomSize);
        Middle.ScaleVec = (vector + new Vector2(Overlap)) / Middle.Size;
        return vector;
    }

    private void RefreshBorderScales(Vector2 middleSize)
    {
        Left.ScaledSize = middleSize.ChangeX(Left.Size.X);
        Right.ScaledSize = middleSize.ChangeX(Right.Size.X);
        Top.ScaledSize = middleSize.ChangeY(Top.Size.Y);
        Bottom.ScaledSize = middleSize.ChangeY(Bottom.Size.Y);
    }

    private void RefreshPositions(Vector2 middleSize)
    {
        Vector2 vector = middleSize / 2f;
        TrySetPosition(LeftTop, -vector);
        TrySetPosition(Top, new Vector2(0f, 0f - vector.Y));
        TrySetPosition(RightTop, new Vector2(vector.X, 0f - vector.Y));
        TrySetPosition(Left, new Vector2(0f - vector.X, 0f));
        TrySetPosition(Right, new Vector2(vector.X, 0f));
        TrySetPosition(LeftBottom, new Vector2(0f - vector.X, vector.Y));
        TrySetPosition(Bottom, new Vector2(0f, vector.Y));
        TrySetPosition(RightBottom, vector);
    }

    private static void TrySetPosition(Sprite node, Vector2 position)
    {
        node?.Position = position;
    }

    private Sprite TryAddChild(Sprite value, Vector2 anchor)
    {
        if (value != null)
        {
            AddChild(value);
            value.Anchor = anchor;
            _childrenAnchors.Add(value, anchor);
        }
        return value;
    }

    private static float MaxWidth(Sprite a, Sprite b, Sprite c)
    {
        return Maths.Max(GetWidth(a), GetWidth(b), GetWidth(c));
    }

    private static float MaxHeight(Sprite a, Sprite b, Sprite c)
    {
        return Maths.Max(GetHeight(a), GetHeight(b), GetHeight(c));
    }

    private static float GetWidth(Sprite sprite)
    {
        return sprite?.Size.X ?? 0f;
    }

    private static float GetHeight(Sprite sprite)
    {
        return sprite?.Size.Y ?? 0f;
    }

    public void ReloadData()
    {
        CallLater(delegate
        {
            RefreshAnchors();
        });
    }
}
