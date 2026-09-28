using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data;

public class TextureNodeData : ConfigData, ITextureNodeData, IConfig
{
    private float scaleFactor = 1f;

    public string Id { get; private set; }

    public string TextureName { get; set; }

    public Texture2D Texture { get; set; }

    public float ScaleFactor
    {
        get => scaleFactor;
        set => scaleFactor = value;
    }

    public TextureNodeData(string id)
    {
        Id = id;
    }
}
