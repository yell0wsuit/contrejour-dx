using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Primitives.Collections;

namespace Mokus2D.Visual.Primitives;

public class LineSprite<T> : SpriteBatchNode where T : struct, ITintVertex
{
	private readonly ISpriteData _spriteData;

	private readonly List<Vector2> _line = new List<Vector2>();

	private readonly List<Pair<Vector2>> _pairs = new List<Pair<Vector2>>();

	private readonly VerticesArray<T> _localVertices = new VerticesArray<T>();

	private readonly VerticesArray<T> _globalVertices = new VerticesArray<T>();

	private bool _pointsDirty;

	private readonly float _width;

	private Color _pointsColor;

	public LineSprite(string id, float width)
	{
		_width = width;
		_spriteData = Mokus2DGame.LoadSpriteData(id);
		base.Texture = _spriteData.Texture;
	}

	public virtual void Clear()
	{
		_line.Clear();
		_globalVertices.Clear();
	}

	public void SetLine(List<Vector2> points)
	{
		_line.Clear();
		_line.AddRange(points);
		_pointsDirty = true;
	}

	protected override void RefreshTransformations(VisualState parentState)
	{
		base.RefreshTransformations(parentState);
		if (_pointsDirty)
		{
			_pairs.Clear();
			PrimitivesUtil.LineToPairs(_line, _pairs, _width);
			_localVertices.Clear();
			_localVertices.SetLength(_pairs.Count * 2);
			PrimitivesUtil.PairsLineToVertices(_pairs, _localVertices.Items);
			_globalVertices.SetLength(_localVertices.Length);
			PrimitivesUtil.FillLineTexture(_globalVertices.Items, _globalVertices.Length, _spriteData);
		}
		if (base.CompositeState.TransformationDirty || _pointsDirty)
		{
			for (int i = 0; i < _localVertices.Length; i++)
			{
				_globalVertices.Items[i].Position = _localVertices.Items[i].Position.Transform(ref base.CompositeState.Matrix);
			}
		}
		_pointsDirty = false;
	}

	protected override void DrawSprite(VisualState state, Color color)
	{
		if (_pointsColor != color)
		{
			RefreshColor(color);
		}
		if (_line.Count > 1)
		{
			int num = _line.Count - 1;
			int indicesCount = num * 6;
			Drawer.Draw(_globalVertices.Items, _globalVertices.Length, PrimitivesIndices.GetLineIndices(num), indicesCount);
		}
	}

	private void RefreshColor(Color color)
	{
		PrimitivesUtil.FillColor(_globalVertices, color, base.CompositeState.ColorRatio);
		_pointsColor = color;
	}
}
