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
    public const int DragCount = 7;

    protected Pair<Vector2> baseAnchors;

    private float baseAngle;

    private Vector2 baseCenter;

    protected Pair<Vector2> basePoints;

    private LevelBuilderBase builder;

    private Touch currentTouch;

    private PlasticineItem dragItem;

    private ContreJourGame game;

    private Vector2 initialPosition;

    private Vector2 initialTouchPosition;

    private Vector2 lastTouchPosition;

    private PlasticineItem left;

    private Pair<Vector2> movingAnchors;

    private PlasticineItem right;

    public DraggingItem(LevelBuilderBase builder, PlasticineItem item, Touch touch)
    {
        currentTouch = touch;
        dragItem = item;
        this.builder = builder;
        game = (ContreJourGame)builder.Game;
        initialTouchPosition = this.builder.TouchRootVec(touch);
        lastTouchPosition = this.builder.TouchRootPoint(touch);
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
        List<Vector2> list = [];
        List<Vector2> list2 =
        [
            basePoints.First,
            baseAnchors.First,
            item,
            topPointsResultWidth.First,
            topPointsResultWidth2.First,
        ];
        BezierUtil.AddBezierPoints(list, list2, 3);
        list2 =
        [
            topPointsResultWidth2.Second,
            topPointsResultWidth.Second,
            item2,
            baseAnchors.Second,
            basePoints.Second,
        ];
        BezierUtil.AddBezierPoints(list, list2, 3);
        for (int i = 0; i < list.Count; i++)
        {
            PlasticineBodyClip.SetDotPositionPosition();
        }
        currentDragPoint.Y += 30f;
        PlasticineBodyClip.SetDotPositionPosition();
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
