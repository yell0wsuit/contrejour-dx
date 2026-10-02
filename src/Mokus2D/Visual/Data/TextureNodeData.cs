using Mokus2D.Graphics;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data
{
    public class TextureNodeData(string id) : ConfigData, ITextureNodeData, IConfig
    {
        public string Id { get; private set; } = id;

        public string TextureName { get; set; }

        public ITexture Texture { get; set; }

        public float ScaleFactor { get; set; } = 1f;
    }
}
