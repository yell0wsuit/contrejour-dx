using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Util;
using Mokus2D.Visual;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Parallax;

public class ParallaxScroller : IViewPosition
{
    public const string ParallaxConfigName = "parallax";

    private Vector2 _viewPosition = Vector2.Zero;

    private readonly Vector2 _screenSize;

    private float _zoomDistance;

    private Vector2 _zoomCenter = Vector2.Zero;

    private readonly List<ParallaxLayer> _layers = [];

    public IReadOnlyList<ParallaxLayer> Layers => _layers;

    private Vector2? _fieldZoomPosition;

    public Vector2 ScreenSize => _screenSize;

    public float ZoomDistance
    {
        get => _zoomDistance;
        set
        {
            if (_zoomDistance != value)
            {
                float mainLayerScale = MainLayerScale;
                _zoomDistance = value;
                RefreshZoom();
                FixZoomPosition(mainLayerScale, MainLayerScale);
                ZoomChanged.Dispatch();
            }
        }
    }

    public float MainLayerScale
    {
        get => 1f / (1f + ZoomDistance);
        set => ZoomDistance = (1f - value) / value;
    }

    public Vector2 CenterPosition
    {
        get => ViewPosition + (_screenSize / MainLayerScale / 2f);
        set => ViewPosition = value - (_screenSize / MainLayerScale / 2f);
    }

    public Vector2 ViewPosition
    {
        get => _viewPosition;
        set
        {
            if (_viewPosition != value)
            {
                _fieldZoomPosition = null;
                _viewPosition = value;
                RefreshViewPosition();
                ViewPositionChanged.Dispatch();
            }
        }
    }

    public RectangleFloat ViewBounds => new(ViewPosition, ScreenSize / MainLayerScale);

    public Vector2 ZoomCenter
    {
        get => _zoomCenter;
        set
        {
            if (_zoomCenter != value)
            {
                _zoomCenter = value;
                _fieldZoomPosition = null;
            }
        }
    }

    public event Action ViewPositionChanged;

    public event Action ZoomChanged;

    public static float GetConfigParallax(Node node)
    {
        return node.Config != null ? node.Config.GetFloat("parallax", 1f) : 1f;
    }

    public ParallaxScroller(Vector2 screenSize)
    {
        _screenSize = screenSize;
        _zoomCenter = screenSize / 2f;
    }

    public ParallaxLayer FindLayer(Node node)
    {
        return _layers.Find(l => l.Node == node);
    }

    public void RemoveLayer(Node node)
    {
        ParallaxLayer item = FindLayer(node);
        _ = _layers.Remove(item);
    }

    public void AddChildrenByConfigs(Node parent)
    {
        foreach (Node child in parent.Children)
        {
            float parallax = 1f;
            if (child != null)
            {
                parallax = child.Config.GetFloat("parallax", 1f);
            }
            Add(child, parallax);
        }
    }

    public void Add(Node layer, float parallax)
    {
        Add(layer, parallax, layer.Position);
    }

    public void Add(Node layer, float parallax, Vector2 initialPosition)
    {
        ParallaxLayer parallaxLayer = new(layer, parallax, initialPosition);
        _layers.Add(parallaxLayer);
        RefreshLayerZoom(parallaxLayer);
        RefreshLayerPosition(parallaxLayer);
    }

    private void RefreshViewPosition()
    {
        foreach (ParallaxLayer layer in _layers)
        {
            RefreshLayerPosition(layer);
        }
    }

    private void RefreshLayerPosition(ParallaxLayer layer)
    {
        Vector2 vector = layer.InitialPosition - (ViewPosition * layer.Parallax);
        float layerScale = GetLayerScale(layer);
        layer.Node.Position = vector * layerScale;
    }

    private void FixZoomPosition(float oldZoom, float newZoom)
    {
        Vector2? fieldZoomPosition = _fieldZoomPosition;
        if (!fieldZoomPosition.HasValue)
        {
            fieldZoomPosition = ViewPosition + (ZoomCenter / oldZoom);
        }
        Vector2 vector = ViewPosition + (ZoomCenter / newZoom);
        ViewPosition += fieldZoomPosition.Value - vector;
        _fieldZoomPosition = fieldZoomPosition;
    }

    private void RefreshZoom()
    {
        foreach (ParallaxLayer layer in _layers)
        {
            RefreshLayerZoom(layer);
        }
        RefreshViewPosition();
    }

    private void RefreshLayerZoom(ParallaxLayer layer)
    {
        layer.Node.ScaleVec = layer.InitialScale * GetLayerScale(layer);
    }

    private float GetLayerScale(ParallaxLayer layer)
    {
        return layer.Parallax == 0f ? 1f : layer.ParallaxDistance / (layer.ParallaxDistance + ZoomDistance);
    }
}
