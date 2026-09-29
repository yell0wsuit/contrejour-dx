using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Interfaces;

public interface ISpriteData : ITextureNodeData, IConfig
{
    Vector2 Anchor { get; }

    Rectangle TextureRect { get; }
}
