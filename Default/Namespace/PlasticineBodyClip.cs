using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Visual;

namespace Default.Namespace;

public class PlasticineBodyClip : ContreJourBodyClip, IRestartable
{
    private const float OPACITY_STEP = 5.1f;

    private const float END_OPACITY = 102f;

    private const int START_OPACITY = 0;

    private const float MAX_DRAG_FORCE = 800f;

    private const int GROUND_FILL_STEP = 2;

    protected bool changed;

    protected PlasticineSprite clipContent;

    protected Dictionary<Touch, DraggingItem> draggingItems;

    protected PlasticineItem firstItem;

    protected PlasticineHighliteBorder highlite;

    protected float lastTouchTime;

    private PlasticineItem leftItem;

    protected PlasticineWideBorder wideBorder;

    public PlasticineItem FirstItem => firstItem;

    public bool Changed
    {
        get
        {
            return changed;
        }
        set
        {
            changed = value;
        }
    }

    public PlasticineBodyClip(LevelBuilderBase _builder, List<Vector2> points, Node _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        ContreJourGame contreJourGame = (ContreJourGame)_builder.Game;
        contreJourGame.RegisterPlasticine(this);
        clipContent = (PlasticineSprite)ReflectUtil.CreateInstance(contreJourGame.ChooseSide(typeof(BlackPlasticineSprite), typeof(WhitePlasticineSprite), typeof(PlasticineSprite)));
        Create(points);
        builder.AddChild(clipContent);
        firstItem.BodyClip.UpdateParent = true;
        wideBorder = new PlasticineWideBorder();
        builder.AddChild(wideBorder);
        InitBorder(contreJourGame);
        InitFillSprite();
        if (!contreJourGame.RoseChapter)
        {
            highlite = new PlasticineHighliteBorder(firstItem, wideBorder);
        }
        if (highlite != null)
        {
            builder.AddChild(highlite);
        }
        changed = false;
        draggingItems = new Dictionary<Touch, DraggingItem>();
    }

    public void Restart()
    {
        PlasticineItem nextItem = firstItem;
        do
        {
            nextItem.BodyClip.MoveToInitialPosition();
            nextItem = nextItem.NextItem;
        }
        while (nextItem != firstItem);
    }

    private void InitFillSprite()
    {
        int num = 0;
        int vertices = 0;
        PlasticineItem nextItem = leftItem.NextItem;
        PlasticineItem previousItem = leftItem.PreviousItem;
        do
        {
            if (num % 2 == 0)
            {
                InitFillSpriteVertices(nextItem, previousItem, ref vertices);
            }
            nextItem = nextItem.NextItem;
            previousItem = previousItem.PreviousItem;
            num++;
        }
        while (nextItem != previousItem && nextItem.NextItem != previousItem);
        if (nextItem == previousItem)
        {
            nextItem.BodyClip.SetFillSprite(clipContent, vertices++);
        }
        else
        {
            InitFillSpriteVertices(nextItem, previousItem, ref vertices);
        }
        clipContent.InitVertices(vertices);
    }

    private void InitFillSpriteVertices(PlasticineItem top, PlasticineItem bottom, ref int vertices)
    {
        top.BodyClip.SetFillSprite(clipContent, vertices);
        bottom.BodyClip.SetFillSprite(clipContent, vertices + 1);
        vertices += 2;
    }

    private void InitBorder(ContreJourGame game)
    {
        PlasticineItem nextItem = firstItem;
        int num = 0;
        do
        {
            nextItem.BodyClip.SetWideBorder(wideBorder, num);
            num++;
            nextItem = nextItem.NextItem;
        }
        while (nextItem != firstItem);
        Color borderOutColor = (game.BlackSide ? PlasticineConstants.BLACK_BORDER_OUT_COLOR : ((!game.WhiteSide) ? new Color(0, 0, 0, 0) : PlasticineConstants.WHITE_GROUND_OUT_COLOR));
        wideBorder.SetSizeBorderColorBorderOutColor(num, clipContent.Color, borderOutColor);
    }

    public PlasticineItem GetClosestItem(Vector2 point)
    {
        PlasticineItem nextItem = firstItem;
        PlasticineItem plasticineItem = null;
        float num = 0f;
        do
        {
            float num2 = (nextItem.Body.Position - point).LengthSquared();
            if (num2 < num || plasticineItem == null)
            {
                plasticineItem = nextItem;
                num = num2;
            }
            nextItem = nextItem.NextItem;
        }
        while (nextItem != firstItem);
        return plasticineItem;
    }

    public bool PointInside(Vector2 point)
    {
        float num = 30f;
        List<Vector2> list = new List<Vector2>();
        list.Add(new Vector2(point.X + num, point.Y));
        list.Add(new Vector2(point.X, point.Y + num));
        list.Add(new Vector2(point.X - num, point.Y));
        list.Add(new Vector2(point.X, point.Y - num));
        for (int i = 0; i < list.Count; i++)
        {
            List<Fixture> list2 = FarseerUtil.Raycast(endPoint: list[i], world: builder.World, startPoint: point);
            bool flag = false;
            for (int j = 0; j < list2.Count; j++)
            {
                if (list2[j].Body.UserData is BodyClip bodyClip && bodyClip is PlasticinePartBodyClip && ((PlasticinePartBodyClip)bodyClip).Parent == this)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
        }
        return true;
    }

    public void GetBorderVerticesOffset(ref List<Vector2> polygon, float offset)
    {
        PlasticineItem nextItem = firstItem;
        do
        {
            polygon.Add(nextItem.GetBorder(offset));
            nextItem = nextItem.NextItem;
        }
        while (nextItem != firstItem);
    }

    public PlasticineBorder CreateOutBorder(float offset)
    {
        List<Vector2> polygon = new List<Vector2>();
        GetBorderVerticesOffset(ref polygon, offset);
        if (!base.Game.BlackSide)
        {
            if (!base.Game.BonusChapter)
            {
                return new PlasticineBorder(polygon);
            }
            return new GreenPlasticineBorder(polygon);
        }
        return new BlackPlasticineBorder(polygon);
    }

    public bool StartDragItemTouch(PlasticineItem item, Touch touch)
    {
        int i = 0;
        PlasticineItem previousItem = item.PreviousItem;
        PlasticineItem nextItem = item.NextItem;
        for (; i < 7; i++)
        {
            if (previousItem.BodyClip.Dragging || nextItem.BodyClip.Dragging)
            {
                return false;
            }
            previousItem = previousItem.PreviousItem;
            nextItem = nextItem.NextItem;
        }
        item.BodyClip.ScareFlyes(0);
        item.NextItem.BodyClip.ScareFlyes(0);
        item.NextItem.NextItem.BodyClip.ScareFlyes(0);
        item.PreviousItem.BodyClip.ScareFlyes(0);
        item.PreviousItem.PreviousItem.BodyClip.ScareFlyes(0);
        DraggingItem value = new DraggingItem(builder, item, touch);
        draggingItems[touch] = value;
        return true;
    }

    public void StopDragTouch(PlasticineItem item, Touch touch)
    {
        DraggingItem draggingItem = draggingItems[touch];
        draggingItems.Remove(touch);
        draggingItem.Finish();
    }

    public void UpdateGraphics(float time)
    {
        lastTouchTime += time;
        PlasticineItem nextItem = firstItem;
        do
        {
            nextItem.BodyClip.UpdateWideBorderAndFill();
            nextItem = nextItem.NextItem;
        }
        while (nextItem != firstItem);
        if (highlite != null)
        {
            highlite.Update(time);
        }
        if (changed)
        {
            changed = false;
        }
    }

    public bool TouchMove(Touch touch)
    {
        changed |= draggingItems[touch].Update();
        lastTouchTime = 0f;
        return true;
    }

    public void SetDotPositionPosition(int index, Vector2 position)
    {
    }

    public void Create(List<Vector2> points)
    {
        firstItem = SurfaceCreator.CreateParentPointsMaxWidth((ContreJourLevelBuilder)builder, this, points, 0.6f, out leftItem);
    }
}
