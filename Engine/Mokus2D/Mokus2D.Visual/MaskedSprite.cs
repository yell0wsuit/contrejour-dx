using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual;

public class MaskedSprite : RenderSprite
{
    protected readonly MaskRoot maskRoot;

    private readonly BlendState blendState;

    private SpriteBatchNode mask;

    public bool UpdateMask
    {
        get
        {
            return maskRoot.UpdateChildren;
        }
        set
        {
            maskRoot.UpdateChildren = value;
        }
    }

    public SpriteBatchNode Mask
    {
        get
        {
            return mask;
        }
        set
        {
            if (mask != null)
            {
                maskRoot.RemoveChild(mask);
            }
            mask = value;
            if (mask != null)
            {
                maskRoot.AddChild(mask);
            }
        }
    }

    public MaskedSprite(AnchorNode mask)
        : this(mask, mask.TextureSize)
    {
        Anchor = mask.Anchor;
    }

    public MaskedSprite(SpriteBatchNode mask, Vector2 size)
        : this(size)
    {
        Mask = mask;
    }

    public MaskedSprite(Vector2 size)
        : base(size)
    {
        blendState = new BlendState();
        blendState.ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.SourceAlpha;
        blendState.AlphaDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.SourceAlpha;
        blendState.ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.Zero;
        blendState.AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.Zero;
        maskRoot = new MaskRoot((int)size.X, (int)size.Y);
    }

    protected override void DrawContent()
    {
        if (mask != null)
        {
            base.DrawContent();
            if (Mokus2DGame.Config.RenderTargetEnabled)
            {
                maskRoot.ScaleX = Math.Sign(base.Root.ScaleX);
                maskRoot.ScaleY = Math.Sign(base.Root.ScaleY);
                maskRoot.SpritesScaleFactor = base.Root.SpritesScaleFactor;
                BlendState blend = mask.Blend;
                mask.Blend = blendState;
                maskRoot.Position = base.AnchorInPixels;
                maskRoot.DrawAll();
                mask.Blend = blend;
            }
        }
    }
}
