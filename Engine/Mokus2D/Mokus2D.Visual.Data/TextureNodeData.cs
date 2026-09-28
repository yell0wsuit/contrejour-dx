using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data;

public class TextureNodeData : ConfigData, ITextureNodeData, IConfig
{
    public string Id { get; private set; }

    public string TextureName { get; set; }

    public Texture2D Texture { get; set; }

    public float ScaleFactor { get; set; } = 1f;

    public TextureNodeData(string id)
    {
        Id = id;
    }
}
