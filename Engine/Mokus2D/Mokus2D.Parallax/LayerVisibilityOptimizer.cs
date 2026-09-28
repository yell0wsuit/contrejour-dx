using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mokus2D.Data;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.Parallax;

public class LayerVisibilityOptimizer
{
	private readonly VisibleIsland[,] _inBoundsIslands;

	private readonly Dictionary<Mokus2D.Util.Data.Point, VisibleIsland> _outOfBoundsIslands = new Dictionary<Mokus2D.Util.Data.Point, VisibleIsland>();

	private readonly Vector2 _islandSize;

	private readonly RectangleFloat _bounds;

	private readonly Mokus2D.Util.Data.Point _size;

	private readonly Mokus2D.Util.Data.Point _maxIndex;

	private HashSet<VisibleIsland> _currentIslands = new HashSet<VisibleIsland>();

	private HashSet<VisibleIsland> _previousIslands = new HashSet<VisibleIsland>();

	private Rectangle? _visibleArea;

	private readonly Node _layer;

	private Mokus2D.Util.Data.Point _leftTopIndex;

	private Mokus2D.Util.Data.Point _rightBottomIndex;

	public Rectangle? VisibleArea
	{
		get
		{
			return _visibleArea;
		}
		set
		{
			if (_visibleArea != value)
			{
				_visibleArea = value;
				RefreshVisibleArea();
			}
		}
	}

	public LayerVisibilityOptimizer(Node layer, RectangleFloat bounds, Vector2 islandSize)
	{
		_layer = layer;
		_bounds = bounds;
		_islandSize = islandSize;
		_size = (Mokus2D.Util.Data.Point)VectorExtensions.Ceiling(bounds.Size / islandSize);
		_maxIndex = _size - new Mokus2D.Util.Data.Point(1);
		_inBoundsIslands = new VisibleIsland[_size.X, _size.Y];
	}

	public void Rebuild()
	{
		foreach (Node child in _layer.Children)
		{
			AddItem(child);
		}
	}

	public void RefreshSingleCellItem(LinkedListNode<Node> item)
	{
		VisibleIsland island = ((VisibleIslandChildren)item.List).Island;
		Mokus2D.Util.Data.Point singleCellItemIndex = GetSingleCellItemIndex(item.Value);
		if (island.GridPosition != singleCellItemIndex)
		{
			island.Remove(item);
			VisibleIsland singleCellItemIsland = GetSingleCellItemIsland(singleCellItemIndex);
			singleCellItemIsland.Add(item);
		}
	}

	public LinkedListNode<Node> AddSingleCellItem(Node child)
	{
		child.OnScreenCount--;
		Mokus2D.Util.Data.Point islandIndex = GetIslandIndex(child);
		VisibleIsland singleCellItemIsland = GetSingleCellItemIsland(islandIndex);
		return singleCellItemIsland.Add(child);
	}

	private VisibleIsland GetSingleCellItemIsland(Mokus2D.Util.Data.Point cellIndex)
	{
		return GetOrCreateIsland(cellIndex);
	}

	private VisibleIsland GetOrCreateIsland(Mokus2D.Util.Data.Point cellIndex)
	{
		VisibleIsland visibleIsland = GetIsland(cellIndex);
		if (visibleIsland == null)
		{
			visibleIsland = new VisibleIsland(cellIndex, cellIndex.Between(_leftTopIndex, _rightBottomIndex) || !_visibleArea.HasValue);
			if (InBounds(cellIndex))
			{
				_inBoundsIslands[cellIndex.X, cellIndex.Y] = visibleIsland;
			}
			else
			{
				_outOfBoundsIslands[cellIndex] = visibleIsland;
			}
		}
		return visibleIsland;
	}

	private VisibleIsland GetIsland(Mokus2D.Util.Data.Point cellIndex)
	{
		if (InBounds(cellIndex))
		{
			return _inBoundsIslands[cellIndex.X, cellIndex.Y];
		}
		return _outOfBoundsIslands.TryGetValue(cellIndex);
	}

	private bool InBounds(Mokus2D.Util.Data.Point cellIndex)
	{
		return cellIndex.Between(Mokus2D.Util.Data.Point.Zero, _maxIndex);
	}

	private Mokus2D.Util.Data.Point GetSingleCellItemIndex(Node child)
	{
		Mokus2D.Util.Data.Point islandIndex = GetIslandIndex(child);
		if (!islandIndex.Between(Mokus2D.Util.Data.Point.Zero, _maxIndex))
		{
			return new Mokus2D.Util.Data.Point(-1);
		}
		return islandIndex;
	}

	private Mokus2D.Util.Data.Point GetIslandIndex(Node child)
	{
		return GetIslandIndex(child.Position);
	}

	private void AddItem(Node child)
	{
		if (child is Sprite sprite)
		{
			AddMultiCellItem(sprite, sprite.Bounds);
		}
		else if (child is AnimationNode animationNode)
		{
			AddMultiCellItem(animationNode, animationNode.PrecalculatedBounds);
		}
		else
		{
			AddSingleCellItem(child);
		}
	}

	private void AddMultiCellItem(Node child, RectangleFloat bounds)
	{
		child.OnScreenCount--;
		Matrix matrix = child.NodeMatrix;
		Mokus2D.Util.Data.Point min = GetIslandIndex(ref matrix, bounds.LeftTop);
		Mokus2D.Util.Data.Point max = min;
		RefreshIslandIndices(ref matrix, bounds.RightBottom, ref min, ref max);
		RefreshIslandIndices(ref matrix, bounds.LeftBottom, ref min, ref max);
		RefreshIslandIndices(ref matrix, bounds.RightTop, ref min, ref max);
		for (int i = min.X; i <= max.X; i++)
		{
			for (int j = min.Y; j <= max.Y; j++)
			{
				VisibleIsland orCreateIsland = GetOrCreateIsland(new Mokus2D.Util.Data.Point(i, j));
				orCreateIsland.Add(child);
			}
		}
	}

	private void RefreshIslandIndices(ref Matrix matrix, Vector2 position, ref Mokus2D.Util.Data.Point min, ref Mokus2D.Util.Data.Point max)
	{
		Mokus2D.Util.Data.Point islandIndex = GetIslandIndex(ref matrix, position);
		min = Mokus2D.Util.Data.Point.Min(min, islandIndex);
		max = Mokus2D.Util.Data.Point.Max(max, islandIndex);
	}

	private Mokus2D.Util.Data.Point GetIslandIndex(ref Matrix matrix, Vector2 position)
	{
		Vector3 position2 = position.ToVector3();
		Vector3.Transform(ref position2, ref matrix, out var result);
		return GetIslandIndex(result.ToVector2());
	}

	private Mokus2D.Util.Data.Point GetIslandIndex(Vector2 corner)
	{
		return (Mokus2D.Util.Data.Point)VectorExtensions.Floor((corner - _bounds.LeftTop) / _islandSize);
	}

	private void RefreshVisibleArea()
	{
		if (VisibleArea.HasValue)
		{
			RefreshVisibleAreaValue();
		}
		else
		{
			ShowAllIslands();
		}
	}

	private void ShowAllIslands()
	{
		_currentIslands.Clear();
		VisibleIsland[,] inBoundsIslands = _inBoundsIslands;
		foreach (VisibleIsland visibleIsland in inBoundsIslands)
		{
			if (visibleIsland != null)
			{
				visibleIsland.Visible = true;
				_currentIslands.Add(visibleIsland);
			}
		}
		foreach (VisibleIsland value in _outOfBoundsIslands.Values)
		{
			value.Visible = true;
			_currentIslands.Add(value);
		}
	}

	private void RefreshVisibleAreaValue()
	{
		Mokus2D.Util.Data.Point islandIndex = GetIslandIndex(_visibleArea.Value.LeftTop());
		Mokus2D.Util.Data.Point islandIndex2 = GetIslandIndex(_visibleArea.Value.RightBottom());
		if (islandIndex == _leftTopIndex && islandIndex2 == _rightBottomIndex)
		{
			return;
		}
		_leftTopIndex = islandIndex;
		_rightBottomIndex = islandIndex2;
		HashSet<VisibleIsland> previousIslands = _previousIslands;
		_previousIslands = _currentIslands;
		_currentIslands = previousIslands;
		_currentIslands.Clear();
		for (int i = _leftTopIndex.X; i <= _rightBottomIndex.X; i++)
		{
			for (int j = _leftTopIndex.Y; j <= _rightBottomIndex.Y; j++)
			{
				VisibleIsland island = GetIsland(new Mokus2D.Util.Data.Point(i, j));
				if (island != null)
				{
					if (_previousIslands.Contains(island))
					{
						_previousIslands.Remove(island);
					}
					else
					{
						island.Visible = true;
					}
					_currentIslands.Add(island);
				}
			}
		}
		foreach (VisibleIsland previousIsland in _previousIslands)
		{
			previousIsland.Visible = false;
		}
		_previousIslands.Clear();
	}
}
