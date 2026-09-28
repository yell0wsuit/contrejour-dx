using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual;

public abstract class MultiframeSprite : Sprite
{
    protected List<FrameData> Frames;

    protected ITextureNodeData Data;

    public int TotalFrames => Frames.Count;

    protected MultiframeSprite(IMovieClipData data)
        : base(data.Texture)
    {
        ResetData(data);
    }

    protected MultiframeSprite(string name)
        : this(Mokus2DGame.LoadResource<IMovieClipData>(name))
    {
    }

    protected MultiframeSprite(ISpriteData data)
        : base(data)
    {
        Frames = new List<FrameData>();
        Frames.Add(new FrameData
        {
            Anchor = data.Anchor,
            Rect = data.TextureRect
        });
        TextureSize = new Vector2(data.TextureRect.Width, data.TextureRect.Height);
        Anchor = data.Anchor;
        ScaleFactor = data.ScaleFactor;
        Data = data;
    }

    protected void ResetData(IMovieClipData data)
    {
        ResetTexture(data.Texture);
        Frames = data.Frames;
        TextureSize = data.Size;
        Anchor = data.Anchor;
        ScaleFactor = data.ScaleFactor;
        Data = data;
    }
}
