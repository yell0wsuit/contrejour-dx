using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data;

public class ClipData : IMovieClipData, ISpriteData, ITextureNodeData, IConfig
{
    private readonly List<FrameData> frames = [];

    private float scaleFactor = 1f;

    public int FramesCount;

    public bool UseSheet;

    public bool Jpg;

    public Vector2 TileData;

    public int Width;

    public int Height;

    public float ScaleFactor
    {
        get => scaleFactor;
        set => scaleFactor = value;
    }

    public string Id { get; private set; }

    Vector2 IMovieClipData.Anchor => Anchor;

    public Rectangle TextureRect => Frames[0].Rect;

    public Vector2 Size
    {
        get => new Vector2(Width, Height);
        set
        {
            Width = (int)value.X;
            Height = (int)value.Y;
        }
    }

    public List<FrameData> Frames => frames;

    public string TextureName
    {
        get => Texture.Name;
        set => Texture.Name = value;
    }

    public Texture2D Texture { get; set; }

    public Vector2 Anchor { get; set; }

    public IDictionary<string, string> Config { get; private set; }

    public void Initialize()
    {
        Anchor = new Vector2(Anchor.X, 1f - Anchor.Y);
        FrameData item = default;
        for (int i = 0; i < TileData.Y; i++)
        {
            for (int j = 0; j < TileData.X; j++)
            {
                Rectangle rect = new(j * Width, i * Height, Width, Height);
                item.Anchor = Vector2.Zero;
                item.Rect = rect;
                Frames.Add(item);
                if (Frames.Count == FramesCount)
                {
                    return;
                }
            }
        }
    }
}
