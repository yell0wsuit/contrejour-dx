using System.Numerics;

using Mokus2D.Util.Data;

namespace Mokus2D.Visual.Interfaces
{
    public interface ISpriteData : ITextureNodeData, IConfig
    {
        Vector2 Anchor { get; }

        Rectangle TextureRect { get; }
    }
}
