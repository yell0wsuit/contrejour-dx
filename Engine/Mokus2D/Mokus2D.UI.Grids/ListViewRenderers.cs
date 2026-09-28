using System;
using System.Collections.Generic;

using Mokus2D.Data;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Mokus2D.UI.Grids;

public class ListViewRenderers<T>
{
    private struct Range(int start, int end)
    {
        internal int Start = start;

        internal int End = end;

        public readonly bool Contains(int index)
        {
            return index.Between(Start, End - 1);
        }

        public static bool operator ==(Range a, Range b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Range a, Range b)
        {
            return !(a == b);
        }

        private readonly bool Equals(Range other)
        {
            return Start == other.Start ? End == other.End : false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Range ? Equals((Range)obj) : false;
        }

        public override readonly int GetHashCode()
        {
            return (Start * 397) ^ End;
        }
    }

    private readonly Pool<Node> _renderersPool;

    private readonly List<Node> _itemRenderers = [];

    private float _position;

    private Range _currentRange;

    private readonly ListView<T> _list;

    public List<Node> ItemRenderers => _itemRenderers;

    public float Position
    {
        get => _position;
        set
        {
            value = value.Clamp(0f, Math.Max(ListItemsCount - 1, 0));
            if (_position != value)
            {
                _position = value;
                RefreshPosition();
            }
        }
    }

    private Range CurrentRange
    {
        get => _currentRange;
        set
        {
            value.Start = Math.Max(value.Start, 0);
            value.End = Math.Min(value.End, ListItemsCount);
            if (_currentRange != value)
            {
                Range currentRange = _currentRange;
                _currentRange = value;
                Refresh(currentRange);
                RenderersChanged.Dispatch();
            }
        }
    }

    private int ListItemsCount
    {
        get
        {
            return _list.Data != null ? _list.Data.Count : 0;
        }
    }

    public event Action RenderersChanged;

    public static ListViewRenderers<T> Create<TItemRenderer>(ListView<T> list) where TItemRenderer : Node, IItemRenderer<T>, new()
    {
        return new ListViewRenderers<T>(() => new TItemRenderer(), list);
    }

    public ListViewRenderers(Func<Node> itemRendererFactory, ListView<T> list)
    {
        _list = list;
        _renderersPool = new Pool<Node>(itemRendererFactory);
    }

    public void RefreshData()
    {
        Clear();
        RefreshPosition();
    }

    public void RefreshPosition()
    {
        CurrentRange = new Range(_position.Floor(), _position.Ceiling() + _list.ItemsCount);
    }

    public void Clear()
    {
        CurrentRange = new Range(0, 0);
    }

    public void RefreshCurrentRenderersData()
    {
        for (int i = 0; i < _itemRenderers.Count; i++)
        {
            IItemRenderer<T> itemRenderer = (IItemRenderer<T>)_itemRenderers[i];
            itemRenderer.SetData(_list.SharedData, _list.Data[i + CurrentRange.Start], i);
        }
    }

    private void Refresh(Range oldRange)
    {
        FreeUnusedRenderers(oldRange);
        int num = 0;
        for (int i = CurrentRange.Start; i < CurrentRange.End; i++)
        {
            if (i < oldRange.Start)
            {
                Node item = NewRenderer(i);
                _itemRenderers.Insert(num, item);
                num++;
            }
            if (i >= oldRange.End)
            {
                Node item2 = NewRenderer(i);
                _itemRenderers.Add(item2);
            }
        }
    }

    private Node NewRenderer(int i)
    {
        Node node = _renderersPool.New();
        if (node.Parent == null)
        {
            _list.ItemsContainer.AddChild(node);
        }
        ((IItemRenderer<T>)node).SetData(_list.SharedData, _list.Data[i], i);
        node.VisibleAndUpdating = true;
        return node;
    }

    private void FreeUnusedRenderers(Range oldRange)
    {
        for (int num = _itemRenderers.Count - 1; num >= 0; num--)
        {
            int index = num + oldRange.Start;
            if (!CurrentRange.Contains(index))
            {
                Node node = _itemRenderers[num];
                node.VisibleAndUpdating = false;
                _renderersPool.Free(node);
                _itemRenderers.RemoveAt(num);
            }
        }
    }
}
