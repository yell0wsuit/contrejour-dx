using System.Collections.Generic;

using Mokus2D.Collections;
using Mokus2D.Data;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.Input;

public class SpriteClicksListener : ITouchListener
{
    private readonly Pool<List<IClickableNode>> _listPool = new(() => []);

    private readonly SortedDictionary<int, ForEachCollection<IClickableNode>> _sprites = [];

    private readonly List<ForEachCollection<IClickableNode>> _spritesByPriority = [];

    private readonly List<IClickableNode> _toRemove = [];

    private readonly Dictionary<Touch, List<IClickableNode>> _touchedSprites = [];

    public SpriteClicksListener()
    {
        Mokus2DGame.Instance.TouchController.AddListener(this);
    }

    public bool TouchBegin(Touch touch)
    {
        List<IClickableNode> list = _listPool.New();
        list.Clear();
        foreach (ForEachCollection<IClickableNode> value in _sprites.Values)
        {
            _spritesByPriority.Add(value);
        }
        foreach (ForEachCollection<IClickableNode> item in _spritesByPriority)
        {
            using (item.Using())
            {
                foreach (IClickableNode item2 in item)
                {
                    if (!touch.Stoped)
                    {
                        Node node = (Node)item2;
                        if (node.RootInteractionsEnabled && SpriteContainsTouch(item2, touch) && item2.TouchBegin(touch))
                        {
                            list.Add(item2);
                        }
                        continue;
                    }
                    break;
                }
            }
        }
        _spritesByPriority.Clear();
        if (!list.Empty())
        {
            _touchedSprites[touch] = list;
            return true;
        }
        _listPool.Free(list);
        return false;
    }

    public bool TouchMove(Touch touch)
    {
        List<IClickableNode> list = _touchedSprites[touch];
        foreach (IClickableNode item in list)
        {
            if (!touch.Stoped)
            {
                bool flag = SpriteContainsTouch(item, touch);
                if ((flag && !item.TouchMove(touch)) || (!flag && !item.TouchOut(touch)))
                {
                    _toRemove.Add(item);
                }
                continue;
            }
            break;
        }
        list.RemoveListNoGarbage(_toRemove);
        _toRemove.Clear();
        return true;
    }

    public void TouchEnd(Touch touch)
    {
        List<IClickableNode> list = _touchedSprites[touch];
        foreach (IClickableNode item in list)
        {
            Node node = (Node)item;
            if (node.RootInteractionsEnabled)
            {
                item.TouchEnd(touch);
            }
        }
        list.Clear();
        _listPool.Free(list);
    }

    public void Add(IClickableNode sprite)
    {
        ForEachCollection<IClickableNode> orCreatePriorityList = GetOrCreatePriorityList(sprite);
        orCreatePriorityList.Add(sprite);
    }

    public void Remove(IClickableNode sprite)
    {
        ForEachCollection<IClickableNode> orCreatePriorityList = GetOrCreatePriorityList(sprite);
        _ = orCreatePriorityList.Remove(sprite);
    }

    private ForEachCollection<IClickableNode> GetOrCreatePriorityList(IClickableNode sprite)
    {
        ForEachCollection<IClickableNode> forEachList = _sprites.TryGetValue(sprite.ClickablePriority);
        if (forEachList == null)
        {
            forEachList = [];
            _sprites.Add(sprite.ClickablePriority, forEachList);
        }
        return forEachList;
    }

    private static bool SpriteContainsTouch(IClickableNode sprite, Touch touch)
    {
        return sprite.ContainsGlobalPosition(touch.Position);
    }
}
