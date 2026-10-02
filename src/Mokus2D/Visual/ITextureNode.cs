using Mokus2D.Graphics;

namespace Mokus2D.Visual
{
    public interface ITextureNode : ISizeNode
    {
        ITexture Texture { get; }
    }
}
