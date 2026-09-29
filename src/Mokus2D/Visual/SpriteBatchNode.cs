using Microsoft.Xna.Framework;

using Mokus2D.Graphics;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual
{
    public abstract class SpriteBatchNode : Node
    {
        private SpriteBatchProperties spriteBatchProperties = Mokus2DGame.Config.DefaultSpriteBatchProperties;

        public ref SpriteBatchProperties SpriteBatchProperties => ref spriteBatchProperties;

        protected float ScaleFactor { get; set; } = 1f;

        public ITexture Texture { get; protected set; }

        public BlendMode Blend
        {
            get => SpriteBatchProperties.Blend;
            set => SpriteBatchProperties.Blend = value;
        }

        protected SpriteBatchNode()
        {
        }

        protected SpriteBatchNode(ITexture texture)
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
}
