using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual
{
    public class LayerColor : SpriteBatchNode, IDataReloadable
    {
        private readonly IQuad _quad;

        private ISpriteData _spriteData;

        public LayerColor(string name)
            : this(Color.Black, name)
        {
        }

        public LayerColor(Color color, string name)
            : this(color, Mokus2DGame.LoadSpriteData(name))
        {
        }

        public LayerColor(Color color, ISpriteData spriteData)
        {
            _quad = Mokus2DGame.Config.GraphicsConfig.CreateDefaultQuad();
            _spriteData = spriteData;
            Texture = _spriteData.Texture;
            Color = color;
            ColorRatio = 1f;
        }

        protected override void OnAddedToStage()
        {
            base.OnAddedToStage();
            RefreshQuad();
        }

        public void RefreshQuad()
        {
            Vector2 size = Root.Size;
            _quad.RefreshColor(Color, ColorRatio);
            _quad.RefreshTextureRect(_spriteData.TextureRect, _spriteData.Texture.Bounds.Size());
            _quad.SetPositions(new Vector2(-10f, -10f), new Vector2(size.X + 10f, size.Y + 10f));
        }

        public override void Draw(VisualState state)
        {
            if (Texture.IsDisposed)
            {
                ReloadData();
            }
            base.Draw(state);
        }

        protected override void DrawSprite(VisualState state, Color color)
        {
            _quad.RefreshColor(color, CompositeState.ColorRatio);
            _quad.Draw(Drawer);
        }

        public void ReloadData()
        {
            _spriteData = Mokus2DGame.LoadSpriteData(_spriteData.Id);
            Texture = _spriteData.Texture;
            if (OnDisplayList)
            {
                RefreshQuad();
            }
        }
    }
}
