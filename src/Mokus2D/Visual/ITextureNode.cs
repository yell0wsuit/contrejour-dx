using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual
{
    public interface ITextureNode : ISizeNode
    {
        Texture2D Texture { get; }
    }
}
