using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual;

public abstract class SpriteBatchNode : Node
{
	public SpriteBatchProperties SpriteBatchProperties = Mokus2DGame.Config.DefaultSpriteBatchProperties;

	protected float ScaleFactor = 1f;

	public Texture2D Texture { get; protected set; }

	public BlendState Blend
	{
		get
		{
			return SpriteBatchProperties.Blend;
		}
		set
		{
			SpriteBatchProperties.Blend = value;
		}
	}

	protected SpriteBatchNode()
	{
	}

	protected SpriteBatchNode(Texture2D texture)
	{
		Texture = texture;
	}

	public override void Draw(VisualState state)
	{
		base.Draw(state);
		Color color = state.GetColor(premultiply: false);
		BeginDraw(state);
		DrawSprite(state, color);
	}

	protected abstract void DrawSprite(VisualState state, Color color);

	protected virtual void BeginDraw(VisualState state)
	{
		Drawer.BeginBatch(Texture, SpriteBatchProperties);
	}
}
