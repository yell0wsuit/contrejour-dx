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
            return Start == other.Start && End == other.End;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is not null && obj is Range && Equals((Range)obj);
        }

        public override readonly int GetHashCode()
        {
            return (Start * 397) ^ End;
        }
    }

    private readonly Pool<Node> _renderersPool;
    private Range _currentRange;

    private readonly ListView<T> _list;

    public List<Node> ItemRenderers { get; } = [];

    public float Position
    {
        get; set
        {
            value = value.Clamp(0f, Math.Max(ListItemsCount - 1, 0));
            if (field != value)
            {
                field = value;
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

    private int ListItemsCount => _list.Data != null ? _list.Data.Count : 0;

    public event Action RenderersChanged;

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
        CurrentRange = new Range(Position.Floor(), Position.Ceiling() + _list.ItemsCount);
    }

    public void Clear()
    {
        CurrentRange = new Range(0, 0);
    }

    public void RefreshCurrentRenderersData()
    {
        for (int i = 0; i < ItemRenderers.Count; i++)
        {
            IItemRenderer<T> itemRenderer = (IItemRenderer<T>)ItemRenderers[i];
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
                ItemRenderers.Insert(num, item);
                num++;
            }
            if (i >= oldRange.End)
            {
                Node item2 = NewRenderer(i);
                ItemRenderers.Add(item2);
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
        for (int num = ItemRenderers.Count - 1; num >= 0; num--)
        {
            int index = num + oldRange.Start;
            if (!CurrentRange.Contains(index))
            {
                Node node = ItemRenderers[num];
                node.VisibleAndUpdating = false;
                _renderersPool.Free(node);
                ItemRenderers.RemoveAt(num);
            }
        }
    }
}

public static class ListViewRenderers
{
    public static ListViewRenderers<T> Create<T, TItemRenderer>(ListView<T> list) where TItemRenderer : Node, IItemRenderer<T>, new()
    {
        return new ListViewRenderers<T>(() => new TItemRenderer(), list);
    }
}
