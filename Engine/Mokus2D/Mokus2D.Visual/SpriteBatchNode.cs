using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;

namespace Mokus2D.Visual;

public abstract class SpriteBatchNode : Node
{
    private SpriteBatchProperties spriteBatchProperties = Mokus2DGame.Config.DefaultSpriteBatchProperties;

    public ref SpriteBatchProperties SpriteBatchProperties => ref spriteBatchProperties;

    protected float ScaleFactor { get; set; } = 1f;

    public Texture2D Texture { get; protected set; }

    public BlendState Blend
    {
        get => SpriteBatchProperties.Blend;
        set => SpriteBatchProperties.Blend = value;
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
