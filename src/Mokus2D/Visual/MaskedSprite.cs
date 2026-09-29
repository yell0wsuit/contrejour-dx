using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual
{
    public class MaskedSprite : RenderSprite
    {
        protected MaskRoot MaskRoot { get; }

        private readonly BlendState blendState;

        public bool UpdateMask
        {
            get => MaskRoot.UpdateChildren;
            set => MaskRoot.UpdateChildren = value;
        }

        public SpriteBatchNode Mask
        {
            get;
            set
            {
                if (field != null)
                {
                    MaskRoot.RemoveChild(field);
                }
                field = value;
                if (field != null)
                {
                    MaskRoot.AddChild(field);
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
            blendState = new BlendState
            {
                ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.SourceAlpha,
                AlphaDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.SourceAlpha,
                ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.Zero,
                AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.Zero
            };
            MaskRoot = new MaskRoot((int)size.X, (int)size.Y);
        }

        protected override void DrawContent()
        {
            if (Mask != null)
            {
                base.DrawContent();
                if (Mokus2DGame.Config.RenderTargetEnabled)
                {
                    MaskRoot.ScaleX = Math.Sign(Root.ScaleX);
                    MaskRoot.ScaleY = Math.Sign(Root.ScaleY);
                    MaskRoot.SpritesScaleFactor = Root.SpritesScaleFactor;
                    BlendState blend = Mask.Blend;
                    Mask.Blend = blendState;
                    MaskRoot.Position = AnchorInPixels;
                    MaskRoot.DrawAll();
                    Mask.Blend = blend;
                }
            }
        }
    }
}
