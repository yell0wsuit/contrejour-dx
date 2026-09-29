using Microsoft.Xna.Framework;

namespace Mokus2D.Visual;

public class AnimatedMaskedSprite : MaskedSprite
{
    public AnimatedMaskedSprite(AnchorNode mask)
        : base(mask)
    {
    }

    public AnimatedMaskedSprite(SpriteBatchNode mask, Vector2 size)
        : base(mask, size)
    {
    }

    public AnimatedMaskedSprite(Vector2 size)
        : base(size)
    {
    }

    public override void Update(float time)
    {
        MaskRoot.UpdateNode(time);
        RenderRoot?.UpdateNode(time);
        RedrawTexture();
    }
}
