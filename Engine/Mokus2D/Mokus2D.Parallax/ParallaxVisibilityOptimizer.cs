using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.Parallax;

public class ParallaxVisibilityOptimizer
{
    private readonly Vector2 _extension = new(0f);

    private readonly ParallaxScroller _scroller;

    private readonly List<LayerAndOptimizer> _layers = [];

    private bool _enabled = true;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
                if (value)
                {
                    RefreshLayers();
                }
                else
                {
                    ShowLayers();
                }
            }
        }
    }

    public ParallaxVisibilityOptimizer(ParallaxScroller scroller)
        : this(scroller, Vector2.Zero)
    {
    }

    public ParallaxVisibilityOptimizer(ParallaxScroller scroller, Vector2 extension)
    {
        _extension = extension;
        _scroller = scroller;
        _scroller.ViewPositionChanged += RefreshLayers;
        _scroller.ZoomChanged += RefreshLayers;
    }

    public LayerVisibilityOptimizer FindOptimizer(Node node)
    {
        foreach (LayerAndOptimizer layer in _layers)
        {
            if (layer.Layer.Node == node)
            {
                return layer.Optimizer;
            }
        }
        return null;
    }

    public void Rebuild(RectangleFloat bounds, Predicate<Node> predicate = null)
    {
        Vector2 islandSize = _scroller.ScreenSize / 3f;
        foreach (ParallaxLayer item in _scroller)
        {
            if (item.Parallax != 0f && predicate.NullOrTrue(item.Node))
            {
                Vector2 position = bounds.LeftTop - (item.InitialPosition / item.Parallax);
                Vector2 size = _scroller.ScreenSize + ((bounds.Size - _scroller.ScreenSize) * item.Parallax);
                LayerVisibilityOptimizer layerVisibilityOptimizer = new(item.Node, new RectangleFloat(position, size), islandSize);
                layerVisibilityOptimizer.Rebuild();
                _layers.Add(new LayerAndOptimizer(item, layerVisibilityOptimizer));
            }
        }
        RefreshLayers();
    }

    private void ShowLayers()
    {
        foreach (LayerAndOptimizer layer in _layers)
        {
            layer.Optimizer.VisibleArea = null;
        }
    }

    private void RefreshLayers()
    {
        if (!Enabled)
        {
            return;
        }
        foreach (LayerAndOptimizer layer in _layers)
        {
            Node node = layer.Layer.Node;
            Vector2 leftTop = (-node.Position / node.ScaleVec) - _extension;
            Vector2 size = (_scroller.ScreenSize / node.ScaleVec) + (_extension * 2f);
            layer.Optimizer.VisibleArea = leftTop.ToRectangle(size);
        }
    }
}
