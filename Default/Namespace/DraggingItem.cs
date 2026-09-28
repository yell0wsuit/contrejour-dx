using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class DraggingItem
{
    private const float FREE_ANGLE_DIFF = (float)Math.PI / 8f;

    private const float ORTO_FREE_DISTANCE = 1f;

    private const float FREE_DISTANCE = 3.3333333f;

    public const int DRAG_COUNT = 7;

    protected Pair<Vector2> baseAnchors;

    protected float baseAngle;

    protected Vector2 baseCenter;

    protected Pair<Vector2> basePoints;

    protected LevelBuilderBase builder;

    protected Touch currentTouch;

    protected PlasticineItem dragItem;

    protected ContreJourGame game;

    protected Vector2 initialPosition;

    protected Vector2 initialTouchPosition;

    protected Vector2 lastTouchPosition;

    protected PlasticineItem left;

    protected Pair<Vector2> movingAnchors;

    protected PlasticineItem right;

    public DraggingItem(LevelBuilderBase _builder, PlasticineItem _item, Touch _touch)
    {
        currentTouch = _touch;
        dragItem = _item;
        builder = _builder;
        game = (ContreJourGame)_builder.Game;
        initialTouchPosition = builder.TouchRootVec(_touch);
        lastTouchPosition = builder.TouchRootPoint(_touch);
        initialPosition = dragItem.Body.Position;
        left = PlasticineUtil.GetItemCountDirection(dragItem, 7, -1);
        right = PlasticineUtil.GetItemCountDirection(dragItem, 7, 1);
        SetDragging(value: true);
    }

    public void SetDragging(bool value)
    {
        PlasticineItem previousItem = dragItem.PreviousItem;
        PlasticineItem nextItem = dragItem.NextItem;
        int i = 0;
        dragItem.BodyClip.Dragging = value;
        for (; i < 7; i++)
        {
            previousItem.BodyClip.Dragging = value;
            nextItem.BodyClip.Dragging = value;
            previousItem = previousItem.PreviousItem;
            nextItem = nextItem.NextItem;
        }
    }

    private bool FixBorderDrag(Vector2 currentTouchPosition)
    {
        Vector2 vector = builder.ToVec(currentTouchPosition);
        float num = vector.DistanceTo(dragItem.Body.Position);
        PlasticineItem previousItem = dragItem;
        bool flag = false;
        do
        {
            previousItem = previousItem.PreviousItem;
            float num2 = previousItem.Body.Position.DistanceTo(vector);
            flag = num2 < num;
            if (flag)
            {
                num = num2;
            }
        }
        while (flag);
        previousItem = previousItem.NextItem;
        if (previousItem == dragItem)
        {
            do
            {
                previousItem = previousItem.NextItem;
                float num3 = previousItem.Body.Position.DistanceTo(vector);
                flag = num3 < num;
                if (flag)
                {
                    num = num3;
                }
            }
            while (flag);
            previousItem = previousItem.PreviousItem;
        }
        if (previousItem != dragItem && Math.Abs(previousItem.BodyClip.InitialAngle - dragItem.BodyClip.InitialAngle) >= (float)Math.PI / 8f)
        {
            return true;
        }
        return false;
    }

    public bool UpdateWithTouch(Touch touch)
    {
        currentTouch = touch;
        return Update();
    }

    public bool Update()
    {
        bool result = false;
        Vector2 vector = builder.TouchRootPoint(currentTouch);
        Vector2 vector2 = builder.ToVec(vector);
        if (vector != lastTouchPosition)
        {
            CalculatePositions();
            lastTouchPosition = vector;
            result = true;
        }
        if (Math.Abs(VectorUtil.Projection(vector2 - dragItem.Body.Position, dragItem.BodyClip.Normal)) > 3.3333333f)
        {
            dragItem.BodyClip.Free(currentTouch);
        }
        return result;
    }

    public void CalculateBasePoints()
    {
        basePoints.First = builder.ToPoint(left.GetRight());
        basePoints.Second = builder.ToPoint(right.GetLeft());
        baseCenter = VectorUtil.Center(basePoints.First, basePoints.Second);
        baseAnchors.First = builder.ToPoint(left.GetNextAnchorPosition());
        baseAnchors.Second = builder.ToPoint(right.GetPreviuosAnchorPosition());
        baseAngle = VectorUtil.Atan2(baseAnchors.First, baseAnchors.Second);
    }

    public Pair<Vector2> GetTopPointsResultWidth(Vector2 top, float width)
    {
        return new Pair<Vector2>
        {
            Second = top + VectorUtil.ToVector(width, baseAngle),
            First = top + VectorUtil.ToVector(width, baseAngle + (float)Math.PI)
        };
    }

    public Vector2 GetCurrentDragPoint()
    {
        return builder.ToPoint(GetCurrentDragPosition());
    }

    public Vector2 GetCurrentDragPosition()
    {
        Vector2 vector = builder.TouchRootVec(currentTouch);
        Vector2 touchPosition = initialPosition;
        touchPosition += vector;
        touchPosition -= initialTouchPosition;
        return dragItem.GetTargetPoint(touchPosition);
    }

    public void CalculatePositions()
    {
        CalculateBasePoints();
        Vector2 currentDragPoint = GetCurrentDragPoint();
        Pair<Vector2> topPointsResultWidth = GetTopPointsResultWidth(currentDragPoint, 1.2f / builder.EngineConfig.SizeMultiplier);
        Pair<Vector2> topPointsResultWidth2 = GetTopPointsResultWidth(currentDragPoint, 0.3f / builder.EngineConfig.SizeMultiplier);
        Vector2 item = VectorUtil.Center(baseAnchors.First, topPointsResultWidth.First);
        Vector2 item2 = VectorUtil.Center(baseAnchors.Second, topPointsResultWidth.Second);
        List<Vector2> list = new List<Vector2>();
        List<Vector2> list2 = new List<Vector2>();
        list2.Add(basePoints.First);
        list2.Add(baseAnchors.First);
        list2.Add(item);
        list2.Add(topPointsResultWidth.First);
        list2.Add(topPointsResultWidth2.First);
        BezierUtil.AddBezierPoints(list, list2, 3);
        list2 = new List<Vector2>();
        list2.Add(topPointsResultWidth2.Second);
        list2.Add(topPointsResultWidth.Second);
        list2.Add(item2);
        list2.Add(baseAnchors.Second);
        list2.Add(basePoints.Second);
        BezierUtil.AddBezierPoints(list, list2, 3);
        for (int i = 0; i < list.Count; i++)
        {
            dragItem.BodyClip.Parent.SetDotPositionPosition(i, list[i]);
        }
        currentDragPoint.Y += 30f;
        dragItem.BodyClip.Parent.SetDotPositionPosition(0, currentDragPoint);
        UpdatePositions(list);
    }

    public void UpdatePositions(List<Vector2> positions)
    {
        int i = 0;
        PlasticineItem nextItem = left.NextItem;
        for (; i < positions.Count - 1; i++)
        {
            Vector2 vector = positions[i];
            Vector2 vector2 = positions[i + 1];
            Vector2 point = VectorUtil.Center(vector, vector2);
            nextItem.SetTargetPointAngle(angle: VectorUtil.Atan2(vector, vector2), touchPosition: builder.ToVec(point));
            nextItem = nextItem.NextItem;
        }
        nextItem = left.NextItem;
        for (i = 0; i < positions.Count; i++)
        {
            nextItem.BodyClip.SetDirty();
            nextItem = nextItem.NextItem;
        }
    }

    public void Finish()
    {
        SetDragging(value: false);
    }
}
