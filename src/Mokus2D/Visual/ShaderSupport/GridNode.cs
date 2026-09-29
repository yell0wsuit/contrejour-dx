using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.ShaderSupport
{
    public class GridNode : Node
    {
        private readonly Vector2 _cellSize;

        private readonly RectangleFloat _bounds;

        private readonly Point _gridSize;

        private readonly GridNodeCell[,] _children;

        private readonly RectangleFloat _screenBounds;

        private readonly List<GridNodeCell> _visibleCells = [];

        private readonly List<GridNodeCell> _toRemove = [];

        private Point LeftTopCell = new(-1, -1);

        private Point RightBottomCell = new(-1, -1);

        public Node DebugRoot { get; set; }

        public GridNode(Vector2 cellSize, RectangleFloat bounds, RectangleFloat screenBounds)
        {
            _cellSize = cellSize;
            _bounds = bounds;
            _screenBounds = screenBounds;
            _gridSize = VectorExtensions.Ceiling(_bounds.Size / _cellSize).ToPoint();
            _children = new GridNodeCell[_gridSize.X, _gridSize.Y];
            TransformationsRefreshedEvent += OnTransformationRefreshed;
        }

        public override void AddChild(Node node, int nodeLayer)
        {
            Point cellIndex = GetCellIndex(node.Position);
            GridNodeCell gridNodeCell = _children[cellIndex.X, cellIndex.Y];
            if (gridNodeCell == null)
            {
                gridNodeCell = new GridNodeCell(cellIndex)
                {
                    Visible = false
                };
                _children[cellIndex.X, cellIndex.Y] = gridNodeCell;
                base.AddChild(gridNodeCell, 0);
                if (cellIndex.Between(LeftTopCell, RightBottomCell))
                {
                    gridNodeCell.Visible = true;
                    _visibleCells.Add(gridNodeCell);
                }
            }
            gridNodeCell.AddChild(node, nodeLayer);
        }

        private Point GetCellIndex(Vector2 position)
        {
            position -= _bounds.LeftTop;
            return (position / _cellSize).ToPoint();
        }

        protected override void DrawWithChildren()
        {
            Draw(CompositeState);
            DrawChildrenCells();
        }

        protected virtual void DrawChildrenCells()
        {
            DrawChildrenCells(LeftTopCell, RightBottomCell);
        }

        protected void DrawChildrenCells(Point leftTopCell, Point rightBottomCell)
        {
            for (int i = leftTopCell.Y; i <= rightBottomCell.Y; i++)
            {
                for (int j = leftTopCell.X; j <= rightBottomCell.X; j++)
                {
                    DrawCell(j, i);
                }
            }
        }

        private void DrawCell(int x, int y)
        {
            _children[x, y]?.DrawNode();
        }

        private Point GetLocalCellIndex(Vector2 position)
        {
            position = (DebugRoot == null) ? GlobalToLocal(position, refreshTransformations: false) : DebugRoot.LocalToNode(position, this, refreshTransformations: false);
            return GetCellIndex(position);
        }

        protected virtual void OnTransformationRefreshed()
        {
            LeftTopCell = GetLocalCellIndex(_screenBounds.LeftTop);
            RightBottomCell = GetLocalCellIndex(_screenBounds.RightBottom);
            foreach (GridNodeCell visibleCell in _visibleCells)
            {
                if (!visibleCell.CellIndex.Between(LeftTopCell, RightBottomCell))
                {
                    visibleCell.Visible = false;
                    _toRemove.Add(visibleCell);
                }
            }
            _visibleCells.RemoveListNoGarbage(_toRemove);
            _toRemove.Clear();
            for (int i = LeftTopCell.X; i <= RightBottomCell.X; i++)
            {
                for (int j = LeftTopCell.Y; j <= RightBottomCell.Y; j++)
                {
                    GridNodeCell gridNodeCell = _children[i, j];
                    if (gridNodeCell != null && !gridNodeCell.Visible)
                    {
                        gridNodeCell.Visible = true;
                        _visibleCells.Add(gridNodeCell);
                    }
                }
            }
        }
    }
}
