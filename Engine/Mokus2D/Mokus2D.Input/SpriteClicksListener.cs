using System.Collections.Generic;
using Mokus2D.Collections;
using Mokus2D.Data;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.Input;

public class SpriteClicksListener : ITouchListener
{
	private readonly Pool<List<IClickableNode>> _listPool = new Pool<List<IClickableNode>>(() => new List<IClickableNode>());

	private readonly SortedDictionary<int, ForEachList<IClickableNode>> _sprites = new SortedDictionary<int, ForEachList<IClickableNode>>();

	private readonly List<ForEachList<IClickableNode>> _spritesByPriority = new List<ForEachList<IClickableNode>>();

	private readonly List<IClickableNode> _toRemove = new List<IClickableNode>();

	private readonly Dictionary<Touch, List<IClickableNode>> _touchedSprites = new Dictionary<Touch, List<IClickableNode>>();

	public SpriteClicksListener()
	{
		Mokus2DGame.Instance.TouchController.AddListener(this);
	}

	public bool TouchBegin(Touch touch)
	{
		List<IClickableNode> list = _listPool.New();
		list.Clear();
		foreach (ForEachList<IClickableNode> value in _sprites.Values)
		{
			_spritesByPriority.Add(value);
		}
		foreach (ForEachList<IClickableNode> item in _spritesByPriority)
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
		ForEachList<IClickableNode> orCreatePriorityList = GetOrCreatePriorityList(sprite);
		orCreatePriorityList.Add(sprite);
	}

	public void Remove(IClickableNode sprite)
	{
		ForEachList<IClickableNode> orCreatePriorityList = GetOrCreatePriorityList(sprite);
		orCreatePriorityList.Remove(sprite);
	}

	private ForEachList<IClickableNode> GetOrCreatePriorityList(IClickableNode sprite)
	{
		ForEachList<IClickableNode> forEachList = _sprites.TryGetValue(sprite.ClickablePriority);
		if (forEachList == null)
		{
			forEachList = new ForEachList<IClickableNode>();
			_sprites.Add(sprite.ClickablePriority, forEachList);
		}
		return forEachList;
	}

	private bool SpriteContainsTouch(IClickableNode sprite, Touch touch)
	{
		return sprite.ContainsGlobalPosition(touch.Position);
	}
}
