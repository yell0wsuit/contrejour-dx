using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Fonts
{
    public class CharData(FontData font, Rectangle textureRect, Vector2 anchor, float width) : ISpriteData, ITextureNodeData, IConfig
    {
        private readonly FontData _font = font;

        public float Width { get; } = width;

        public string Id => _font.Id;

        public IDictionary<string, string> Config => null;

        public string TextureName => _font.TextureName;

        public ITexture Texture
        {
            get => _font.Texture;
            set => throw new NotImplementedException();
        }

        public float ScaleFactor => _font.ScaleFactor;

        public Vector2 Anchor { get; private set; } = new Vector2(anchor.X / textureRect.Width, anchor.Y / textureRect.Height);

        public Rectangle TextureRect { get; private set; } = textureRect;
    }
}
