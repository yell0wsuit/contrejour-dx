using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual;

public class OneFrameSprite : Sprite
{
    private FrameData frameData;

    public FrameData FrameData
    {
        get
        {
            return frameData;
        }
        set
        {
            frameData = value;
        }
    }

    public OneFrameSprite(IMovieClipData data, int frame)
        : base(data.Texture)
    {
        Initialize(data, frame);
    }

    public OneFrameSprite(Texture2D texture, FrameData frameData)
        : this(texture, frameData.Rect.Size(), frameData)
    {
    }

    public OneFrameSprite(Texture2D texture, Vector2 textureSize, FrameData frameData)
        : base(texture)
    {
        this.frameData = frameData;
        TextureSize = textureSize;
        Anchor = frameData.Anchor;
    }

    public OneFrameSprite(string name, int frame)
        : this(Mokus2DGame.LoadResource<IMovieClipData>(name), frame)
    {
    }

    public OneFrameSprite(ISpriteData data)
        : base(data)
    {
    }

    protected void Initialize(IMovieClipData data, int frame)
    {
        frameData = data.Frames[frame];
        TextureSize = data.Size;
        Anchor = data.Anchor;
        ScaleFactor = data.ScaleFactor;
    }

    protected override Rectangle GetTileRectangle()
    {
        return frameData.Rect;
    }
}
