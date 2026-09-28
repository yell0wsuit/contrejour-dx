using System;
using System.Collections.Generic;
using Mokus2D.Util.Data;
using Mokus2D.Visual;

namespace Mokus2D.Parallax;

public class VisibleIsland
{
	private bool _visible;

	private readonly VisibleIslandChildren _children;

	public readonly Point GridPosition;

	private bool _inLoop;

	public bool Visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (_visible != value)
			{
				_visible = value;
				RefreshChildren(value ? 1 : (-1));
			}
		}
	}

	public VisibleIsland(Point gridPosition, bool visible)
	{
		GridPosition = gridPosition;
		_children = new VisibleIslandChildren(this);
		_visible = visible;
	}

	private void RefreshChildren(int difference)
	{
		_inLoop = true;
		foreach (Node child in _children)
		{
			child.OnScreenCount += difference;
		}
		_inLoop = false;
	}

	public void Add(LinkedListNode<Node> item)
	{
		ThrowIfInLoop();
		if (Visible)
		{
			item.Value.OnScreenCount++;
		}
		_children.AddLast(item);
	}

	public LinkedListNode<Node> Add(Node child)
	{
		ThrowIfInLoop();
		if (Visible)
		{
			child.OnScreenCount++;
		}
		return _children.AddLast(child);
	}

	public void Remove(LinkedListNode<Node> item)
	{
		ThrowIfInLoop();
		if (Visible)
		{
			item.Value.OnScreenCount--;
		}
		_children.Remove(item);
	}

	private void ThrowIfInLoop()
	{
		if (_inLoop)
		{
			throw new Exception("Cannot modify children while in loop");
		}
	}
}
