using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mokus2D.Data;
using Mokus2D.Util;
using Mokus2D.Visual;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Parallax;

public class ParallaxScroller : IEnumerable<ParallaxLayer>, IEnumerable, IViewPosition
{
	public const string ParallaxConfigName = "parallax";

	private Vector2 _viewPosition = Vector2.Zero;

	private readonly Vector2 _screenSize;

	private float _zoomDistance;

	private Vector2 _zoomCenter = Vector2.Zero;

	private readonly List<ParallaxLayer> _layers = new List<ParallaxLayer>();

	private Vector2? _fieldZoomPosition;

	public Vector2 ScreenSize => _screenSize;

	public float ZoomDistance
	{
		get
		{
			return _zoomDistance;
		}
		set
		{
			if (_zoomDistance != value)
			{
				float mainLayerScale = MainLayerScale;
				_zoomDistance = value;
				RefreshZoom();
				FixZoomPosition(mainLayerScale, MainLayerScale);
				this.ZoomChanged.Dispatch();
			}
		}
	}

	public float MainLayerScale
	{
		get
		{
			return 1f / (1f + ZoomDistance);
		}
		set
		{
			ZoomDistance = (1f - value) / value;
		}
	}

	public Vector2 CenterPosition
	{
		get
		{
			return ViewPosition + _screenSize / MainLayerScale / 2f;
		}
		set
		{
			ViewPosition = value - _screenSize / MainLayerScale / 2f;
		}
	}

	public Vector2 ViewPosition
	{
		get
		{
			return _viewPosition;
		}
		set
		{
			if (_viewPosition != value)
			{
				_fieldZoomPosition = null;
				_viewPosition = value;
				RefreshViewPosition();
				this.ViewPositionChanged.Dispatch();
			}
		}
	}

	public RectangleFloat ViewBounds => new RectangleFloat(ViewPosition, ScreenSize / MainLayerScale);

	public Vector2 ZoomCenter
	{
		get
		{
			return _zoomCenter;
		}
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
		if (node.Config != null)
		{
			return node.Config.GetFloat("parallax", 1f);
		}
		return 1f;
	}

	public ParallaxScroller(Vector2 screenSize)
	{
		_screenSize = screenSize;
		_zoomCenter = screenSize / 2f;
	}

	public ParallaxLayer FindLayer(Node node)
	{
		return _layers.Find((ParallaxLayer l) => l.Node == node);
	}

	public void RemoveLayer(Node node)
	{
		ParallaxLayer item = FindLayer(node);
		_layers.Remove(item);
	}

	public void AddChildrenByConfigs(Node parent)
	{
		foreach (Node child in parent.Children)
		{
			float parallax = 1f;
			IConfig config = child;
			if (config != null)
			{
				parallax = config.Config.GetFloat("parallax", 1f);
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
		ParallaxLayer parallaxLayer = new ParallaxLayer(layer, parallax, initialPosition);
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
		Vector2 vector = layer.InitialPosition - ViewPosition * layer.Parallax;
		float layerScale = GetLayerScale(layer);
		layer.Node.Position = vector * layerScale;
	}

	private void FixZoomPosition(float oldZoom, float newZoom)
	{
		Vector2? fieldZoomPosition = _fieldZoomPosition;
		if (!fieldZoomPosition.HasValue)
		{
			fieldZoomPosition = ViewPosition + ZoomCenter / oldZoom;
		}
		Vector2 vector = ViewPosition + ZoomCenter / newZoom;
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
		if (layer.Parallax == 0f)
		{
			return 1f;
		}
		return layer.ParallaxDistance / (layer.ParallaxDistance + ZoomDistance);
	}

	public IEnumerator<ParallaxLayer> GetEnumerator()
	{
		return _layers.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
