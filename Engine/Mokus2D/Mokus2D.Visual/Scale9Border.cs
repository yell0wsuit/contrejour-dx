using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mokus2D.Effects.Tweening;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual;

public class Scale9Border : Node
{
	public static GetSetValue<Scale9Border, Vector2> SizeValue = new GetSetValue<Scale9Border, Vector2>((Scale9Border n) => n.Size, delegate(Scale9Border n, Vector2 v)
	{
		n.Size = v;
	});

	private static readonly Vector2 CornerAnchor = new Vector2(1f, 0f);

	protected readonly List<ISizeNode> SideSprites = new List<ISizeNode>();

	protected readonly List<ISizeNode> CornerSprites = new List<ISizeNode>();

	private readonly ISizeNode _left;

	private readonly ISizeNode _leftTop;

	private readonly ISizeNode _top;

	private readonly ISizeNode _rightTop;

	private readonly ISizeNode _right;

	private readonly ISizeNode _rightBottom;

	private readonly ISizeNode _bottom;

	private readonly ISizeNode _leftBottom;

	private Vector2 _size;

	private float _borderWidth;

	public float BorderWidth
	{
		get
		{
			return _borderWidth;
		}
		set
		{
			if (_borderWidth != value)
			{
				_borderWidth = value;
				RefreshBorderWidth();
			}
		}
	}

	public Vector2 Size
	{
		get
		{
			return _size;
		}
		set
		{
			if (_size != value)
			{
				_size = value;
				RefreshSize();
			}
		}
	}

	public static Scale9Border CreateForAnimation<TCorner, TSide>(Vector2 size) where TCorner : AnimationNode, new() where TSide : AnimationNode, new()
	{
		return new Scale9Border(size, () => new TCorner(), () => new TSide());
	}

	public Scale9Border(Vector2 size, string cornerSpriteId, string sideSpriteId)
		: this(size, () => new Sprite(cornerSpriteId)
		{
			Anchor = CornerAnchor
		}, () => new Sprite(sideSpriteId)
		{
			Anchor = Vector2.Zero
		})
	{
	}

	public Scale9Border(Vector2 size, Func<ISizeNode> cornerSpriteFactory, Func<ISizeNode> sideSpriteFactory)
	{
		_left = AddSideSprite(sideSpriteFactory, (float)Math.PI / 2f);
		_leftTop = AddCornerSprite(cornerSpriteFactory, (float)Math.PI / 2f);
		_top = AddSideSprite(sideSpriteFactory, (float)Math.PI);
		_rightTop = AddCornerSprite(cornerSpriteFactory, (float)Math.PI);
		_right = AddSideSprite(sideSpriteFactory, -(float)Math.PI / 2f);
		_rightBottom = AddCornerSprite(cornerSpriteFactory, -(float)Math.PI / 2f);
		_bottom = AddSideSprite(sideSpriteFactory, 0f);
		_leftBottom = AddCornerSprite(cornerSpriteFactory, 0f);
		Size = size;
	}

	private ISizeNode AddCornerSprite(Func<ISizeNode> factory, float rotation)
	{
		ISizeNode sizeNode = AddSprite(factory, rotation);
		CornerSprites.Add(sizeNode);
		return sizeNode;
	}

	private ISizeNode AddSideSprite(Func<ISizeNode> factory, float rotation)
	{
		ISizeNode sizeNode = AddSprite(factory, rotation);
		SideSprites.Add(sizeNode);
		return sizeNode;
	}

	private ISizeNode AddSprite(Func<ISizeNode> nodeFactory, float rotation)
	{
		ISizeNode sizeNode = nodeFactory();
		Node node = (Node)sizeNode;
		node.RotationRadians = rotation;
		AddChild(node);
		return sizeNode;
	}

	private void RefreshBorderWidth()
	{
		Vector2 value = new Vector2(_borderWidth);
		foreach (ISizeNode cornerSprite in CornerSprites)
		{
			cornerSprite.SetScaledSize(value);
		}
		foreach (ISizeNode sideSprite in SideSprites)
		{
			((Node)sideSprite).ScaleY = _borderWidth / sideSprite.Size.Y;
		}
	}

	protected virtual void RefreshSize()
	{
		RefreshGroupSize((Node)_leftTop, (Node)_left, Vector2.Zero, Size.Y);
		RefreshGroupSize((Node)_leftBottom, (Node)_bottom, new Vector2(0f, Size.Y), Size.X);
		RefreshGroupSize((Node)_rightBottom, (Node)_right, Size, Size.Y);
		RefreshGroupSize((Node)_rightTop, (Node)_top, new Vector2(Size.X, 0f), Size.X);
	}

	private void RefreshGroupSize(Node corner, Node side, Vector2 position, float sideSize)
	{
		corner.Position = position;
		side.Position = position;
		side.ScaleX = sideSize / ((ISizeNode)side).Size.X;
	}
}
