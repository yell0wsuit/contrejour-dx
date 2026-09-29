using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Config.Tint;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Shaders.Parallax
{
    public class ParallaxSprite : Sprite, IParallaxSprite
    {
        public float Parallax { get; set; }

        public ParallaxSprite(string name)
            : base(name)
        {
            Parallax = 1f;
        }

        public ParallaxSprite(ISpriteData data)
            : base(data)
        {
            Parallax = 1f;
        }

        public ParallaxSprite(Texture2D texture)
            : base(texture)
        {
            Parallax = 1f;
        }

        protected override IQuad CreateQuad()
        {
            return new TintQuad<ParallaxVertex>();
        }

        protected override void RefreshQuad()
        {
            base.RefreshQuad();
            TintQuad<ParallaxVertex> tintQuad = (TintQuad<ParallaxVertex>)Quad;
            tintQuad.LeftTop.Parallax = Parallax;
            tintQuad.RightBottom.Parallax = Parallax;
            tintQuad.RightTop.Parallax = Parallax;
            tintQuad.LeftBottom.Parallax = Parallax;
        }

        protected override void DrawSprite(VisualState state, Color color)
        {
            base.DrawSprite(state, color);
            _ = this.GetRootScale();
        }
    }
}
