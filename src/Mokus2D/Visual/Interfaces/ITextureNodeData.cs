using Mokus2D.Graphics;

namespace Mokus2D.Visual.Interfaces
{
    public interface ITextureNodeData : IConfig
    {
        string Id { get; }

        string TextureName { get; }

        ITexture Texture { get; set; }

        float ScaleFactor { get; }
    }
}
