using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.Parallax
{
    public class LayerVisibilityOptimizer
    {
        private readonly VisibleIsland[,] _inBoundsIslands;

        private readonly Dictionary<Util.Data.Point, VisibleIsland> _outOfBoundsIslands = [];

        private readonly Vector2 _islandSize;

        private readonly RectangleFloat _bounds;

        private readonly Util.Data.Point _size;

        private readonly Util.Data.Point _maxIndex;

        private HashSet<VisibleIsland> _currentIslands = [];

        private HashSet<VisibleIsland> _previousIslands = [];

        private Rectangle? _visibleArea;

        private readonly Node _layer;

        private Util.Data.Point _leftTopIndex;

        private Util.Data.Point _rightBottomIndex;

        public Rectangle? VisibleArea
        {
            get => _visibleArea;
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
            _size = (Util.Data.Point)Vector2.Ceiling(bounds.Size / islandSize);
            _maxIndex = _size - new Util.Data.Point(1);
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
            Util.Data.Point singleCellItemIndex = GetSingleCellItemIndex(item.Value);
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
            Util.Data.Point islandIndex = GetIslandIndex(child);
            VisibleIsland singleCellItemIsland = GetSingleCellItemIsland(islandIndex);
            return singleCellItemIsland.Add(child);
        }

        private VisibleIsland GetSingleCellItemIsland(Util.Data.Point cellIndex)
        {
            return GetOrCreateIsland(cellIndex);
        }

        private VisibleIsland GetOrCreateIsland(Util.Data.Point cellIndex)
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

        private VisibleIsland GetIsland(Util.Data.Point cellIndex)
        {
            return InBounds(cellIndex) ? _inBoundsIslands[cellIndex.X, cellIndex.Y] : _outOfBoundsIslands.GetValueOrDefault(cellIndex);
        }

        private bool InBounds(Util.Data.Point cellIndex)
        {
            return cellIndex.Between(Util.Data.Point.Zero, _maxIndex);
        }

        private Util.Data.Point GetSingleCellItemIndex(Node child)
        {
            Util.Data.Point islandIndex = GetIslandIndex(child);
            return !islandIndex.Between(Util.Data.Point.Zero, _maxIndex) ? new Util.Data.Point(-1) : islandIndex;
        }

        private Util.Data.Point GetIslandIndex(Node child)
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
                _ = AddSingleCellItem(child);
            }
        }

        private void AddMultiCellItem(Node child, RectangleFloat bounds)
        {
            child.OnScreenCount--;
            Matrix matrix = child.NodeMatrix;
            Util.Data.Point min = GetIslandIndex(ref matrix, bounds.LeftTop);
            Util.Data.Point max = min;
            RefreshIslandIndices(ref matrix, bounds.RightBottom, ref min, ref max);
            RefreshIslandIndices(ref matrix, bounds.LeftBottom, ref min, ref max);
            RefreshIslandIndices(ref matrix, bounds.RightTop, ref min, ref max);
            for (int i = min.X; i <= max.X; i++)
            {
                for (int j = min.Y; j <= max.Y; j++)
                {
                    VisibleIsland orCreateIsland = GetOrCreateIsland(new Util.Data.Point(i, j));
                    _ = orCreateIsland.Add(child);
                }
            }
        }

        private void RefreshIslandIndices(ref Matrix matrix, Vector2 position, ref Util.Data.Point min, ref Util.Data.Point max)
        {
            Util.Data.Point islandIndex = GetIslandIndex(ref matrix, position);
            min = Util.Data.Point.Min(min, islandIndex);
            max = Util.Data.Point.Max(max, islandIndex);
        }

        private Util.Data.Point GetIslandIndex(ref Matrix matrix, Vector2 position)
        {
            Vector3 position2 = new(position, 0f);
            Vector3.Transform(ref position2, ref matrix, out Vector3 result);
            return GetIslandIndex(result.ToVector2());
        }

        private Util.Data.Point GetIslandIndex(Vector2 corner)
        {
            return (Util.Data.Point)Vector2.Floor((corner - _bounds.LeftTop) / _islandSize);
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
                    _ = _currentIslands.Add(visibleIsland);
                }
            }
            foreach (VisibleIsland value in _outOfBoundsIslands.Values)
            {
                value.Visible = true;
                _ = _currentIslands.Add(value);
            }
        }

        private void RefreshVisibleAreaValue()
        {
            Util.Data.Point islandIndex = GetIslandIndex(_visibleArea.Value.LeftTop());
            Util.Data.Point islandIndex2 = GetIslandIndex(_visibleArea.Value.RightBottom());
            if (islandIndex == _leftTopIndex && islandIndex2 == _rightBottomIndex)
            {
                return;
            }
            _leftTopIndex = islandIndex;
            _rightBottomIndex = islandIndex2;
            (_currentIslands, _previousIslands) = (_previousIslands, _currentIslands);
            _currentIslands.Clear();
            for (int i = _leftTopIndex.X; i <= _rightBottomIndex.X; i++)
            {
                for (int j = _leftTopIndex.Y; j <= _rightBottomIndex.Y; j++)
                {
                    VisibleIsland island = GetIsland(new Util.Data.Point(i, j));
                    if (island != null)
                    {
                        if (_previousIslands.Contains(island))
                        {
                            _ = _previousIslands.Remove(island);
                        }
                        else
                        {
                            island.Visible = true;
                        }
                        _ = _currentIslands.Add(island);
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
}
