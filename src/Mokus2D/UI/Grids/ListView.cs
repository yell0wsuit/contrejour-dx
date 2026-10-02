using System;
using System.Collections.Generic;

using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Mokus2D.UI.Grids
{
    public class ListView<T> : Node, IListView
    {
        private readonly ListViewRenderers<T> _renderers;
        public Node ItemsContainer { get; } = new();

        private readonly ListViewLayout<T> _layout;
        public object SharedData { get; set; }

        public int ItemsCount { get; private set; }

        public float Height { get; }

        public int DataCount => Data != null ? Data.Count : 0;

        public IList<T> Data
        {
            get; set
            {
                field = value;
                RefreshDataAndDispatchChange();
            }
        } = [];

        public List<Node> ItemRenderers => _renderers.ItemRenderers;

        public float ItemsPosition
        {
            get; set
            {
                if (field != value)
                {
                    field = value;
                    RefreshPosition(value);
                }
            }
        }

        private float TopRendererOffset => 0f - ItemsPosition.Fraction();

        public event Action DataChangedEvent;

        public ListView(Func<Node> itemRendererFactory, int itemsCount, float height)
        {
            SharedData = this;
            ItemsCount = itemsCount;
            Height = height;
            _renderers = new ListViewRenderers<T>(itemRendererFactory, this);
            _renderers.RenderersChanged += OnRenderersChanged;
            _layout = new ListViewLayout<T>(this, ItemsContainer)
            {
                FixedSize = height / itemsCount
            };
            AddChild(ItemsContainer);
        }

        private void RefreshDataAndDispatchChange()
        {
            RefreshData();
            DataChangedEvent.Dispatch();
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
                itemRenderer.RefreshPosition(topOffset + i, ItemsCount);
            }
        }

        private void OnRenderersChanged()
        {
            _layout.Apply();
            RefreshRenderersPosition(TopRendererOffset);
        }
    }

}
