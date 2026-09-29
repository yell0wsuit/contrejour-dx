using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Interfaces;

public interface ITextureNodeData : IConfig
{
    string Id { get; }

    string TextureName { get; }

    Texture2D Texture { get; set; }

    float ScaleFactor { get; }
}
