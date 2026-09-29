using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Visual;

namespace Default.Namespace;

public class PlasticineBodyClip : ContreJourBodyClip, IRestartable
{
    private bool changed;

    private readonly PlasticineSprite clipContent;

    private readonly Dictionary<Touch, DraggingItem> draggingItems;

    private PlasticineItem firstItem;

    private readonly PlasticineHighliteBorder highlite;

    private PlasticineItem leftItem;

    private readonly PlasticineWideBorder wideBorder;

    public PlasticineItem FirstItem => firstItem;

    public bool Changed
    {
        get => changed;
        set => changed = value;
    }

    public PlasticineBodyClip(LevelBuilderBase builder, List<Vector2> points, Node clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        ContreJourGame contreJourGame = (ContreJourGame)builder.Game;
        contreJourGame.RegisterPlasticine(this);
        clipContent = (PlasticineSprite)ReflectUtil.CreateInstance(contreJourGame.ChooseSide(typeof(BlackPlasticineSprite), typeof(WhitePlasticineSprite), typeof(PlasticineSprite)));
        Create(points);
        _ = this.builder.AddChild(clipContent);
        firstItem.BodyClip.UpdateParent = true;
        wideBorder = new PlasticineWideBorder();
        _ = this.builder.AddChild(wideBorder);
        InitBorder(contreJourGame);
        InitFillSprite();
        if (!contreJourGame.RoseChapter)
        {
            highlite = new PlasticineHighliteBorder(firstItem, wideBorder);
        }
        if (highlite != null)
        {
            _ = this.builder.AddChild(highlite);
        }
        changed = false;
        draggingItems = [];
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
        Color borderOutColor = game.BlackSide ? PlasticineConstants.BlackBorderOutColor : ((!game.WhiteSide) ? new Color(0, 0, 0, 0) : PlasticineConstants.WhiteGroundOutColor);
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
        List<Vector2> list =
        [
            new Vector2(point.X + num, point.Y),
            new Vector2(point.X, point.Y + num),
            new Vector2(point.X - num, point.Y),
            new Vector2(point.X, point.Y - num),
        ];
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
        List<Vector2> polygon = [];
        GetBorderVerticesOffset(ref polygon, offset);
        return !Game.BlackSide
            ? !Game.BonusChapter ? new PlasticineBorder(polygon) : new GreenPlasticineBorder(polygon)
            : new BlackPlasticineBorder(polygon);
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
        DraggingItem value = new(builder, item, touch);
        draggingItems[touch] = value;
        return true;
    }

    public void StopDragTouch(Touch touch)
    {
        DraggingItem draggingItem = draggingItems[touch];
        _ = draggingItems.Remove(touch);
        draggingItem.Finish();
    }

    public void UpdateGraphics(float time)
    {
        PlasticineItem nextItem = firstItem;
        do
        {
            nextItem.BodyClip.UpdateWideBorderAndFill();
            nextItem = nextItem.NextItem;
        }
        while (nextItem != firstItem);
        highlite?.Update(time);
        if (changed)
        {
            changed = false;
        }
    }

    public bool TouchMove(Touch touch)
    {
        changed |= draggingItems[touch].Update();
        return true;
    }

    public static void SetDotPositionPosition()
    {
    }

    public void Create(List<Vector2> points)
    {
        firstItem = SurfaceCreator.CreateParentPointsMaxWidth((ContreJourLevelBuilder)builder, this, points, 0.6f, out leftItem);
    }
}
