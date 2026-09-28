using System;
using System.Collections.Generic;

using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Mokus2D.UI.Grids;

public class ListView<T> : Node, IListView
{
    private readonly ListViewRenderers<T> _renderers;

    private float _itemsPosition;

    private readonly float _height;

    public readonly Node ItemsContainer = new Node();

    private readonly ListViewLayout<T> _layout;

    private IList<T> _data = new List<T>();

    public object SharedData;

    public int ItemsCount { get; private set; }

    public float Height => _height;

    public int DataCount
    {
        get
        {
            if (Data != null)
            {
                return Data.Count;
            }
            return 0;
        }
    }

    public IList<T> Data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
            RefreshDataAndDispatchChange();
        }
    }

    public List<Node> ItemRenderers => _renderers.ItemRenderers;

    public float ItemsPosition
    {
        get
        {
            return _itemsPosition;
        }
        set
        {
            if (_itemsPosition != value)
            {
                _itemsPosition = value;
                RefreshPosition(value);
            }
        }
    }

    private float TopRendererOffset => 0f - ItemsPosition.Fraction();

    public event Action DataChangedEvent;

    public static ListView<T> Create(Type rendererType, string animationId, float height)
    {
        int itemsCout = GetItemsCout(animationId, height);
        return new ListView<T>(rendererType, itemsCout, height);
    }

    protected static int GetItemsCout(string animationId, float height)
    {
        AnimationData animationData = Mokus2DGame.LoadAnimation(animationId);
        return (int)(height / (float)animationData.PrecalculatedBounds.Height);
    }

    public ListView(Type itemRendererType, int itemsCount, float height)
        : this((Func<Node>)(() => (Node)Activator.CreateInstance(itemRendererType)), itemsCount, height)
    {
        SharedData = this;
    }

    public ListView(Func<Node> itemRendererFactory, int itemsCount, float height)
    {
        SharedData = this;
        ItemsCount = itemsCount;
        _height = height;
        _renderers = new ListViewRenderers<T>(itemRendererFactory, this);
        _renderers.RenderersChanged += OnRenderersChanged;
        _layout = new ListViewLayout<T>(this, ItemsContainer);
        _layout.FixedSize = height / (float)itemsCount;
        AddChild(ItemsContainer);
    }

    private void RefreshDataAndDispatchChange()
    {
        RefreshData();
        this.DataChangedEvent.Dispatch();
    }

    public void Add(T item)
    {
        Data.Add(item);
        RefreshDataAndDispatchChange();
    }

    public void Remove(T item)
    {
        if (Data.Remove(item))
        {
            RefreshDataAndDispatchChange();
        }
    }

    public void RefreshVisibleItemsData()
    {
        _renderers.RefreshCurrentRenderersData();
    }

    public void Clear()
    {
        Data.Clear();
        RefreshData();
    }

    public void RefreshData()
    {
        _renderers.RefreshData();
    }

    public void SetItemsPositionToEnd()
    {
        ItemsPosition = Math.Max(DataCount - ItemsCount, 0);
    }

    private void RefreshPosition(float value)
    {
        float topRendererOffset = TopRendererOffset;
        ItemsContainer.Y = _layout.FixedSize.Value * topRendererOffset;
        _renderers.Position = value;
        RefreshRenderersPosition(topRendererOffset);
    }

    private void RefreshRenderersPosition(float topOffset)
    {
        for (int i = 0; i < _renderers.ItemRenderers.Count; i++)
        {
            IItemRenderer<T> itemRenderer = (IItemRenderer<T>)_renderers.ItemRenderers[i];
            itemRenderer.RefreshPosition(topOffset + (float)i, ItemsCount);
        }
    }

    private void OnRenderersChanged()
    {
        _layout.Apply();
        RefreshRenderersPosition(TopRendererOffset);
    }
}
