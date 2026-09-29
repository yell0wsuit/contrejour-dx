using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data
{
    public class ClipData : IMovieClipData, ISpriteData, ITextureNodeData, IConfig
    {
        public int FramesCount { get; set; }
        public Vector2 TileData { get; set; }

        private int Width;

        private int Height;

        public float ScaleFactor { get; set; } = 1f;

        public string Id { get; private set; }

        Vector2 IMovieClipData.Anchor => Anchor;

        public Rectangle TextureRect => Frames[0].Rect;

        public Vector2 Size
        {
            get => new(Width, Height);
            set
            {
                Width = (int)value.X;
                Height = (int)value.Y;
            }
        }

        public List<FrameData> Frames { get; } = [];

        public string TextureName
        {
            get => Texture.Name;
            set => Texture.Name = value;
        }

        public ITexture Texture { get; set; }

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
}
