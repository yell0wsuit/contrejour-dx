using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class WhitePlasticineSprite : PlasticineSprite
{
    public override Color Color => PlasticineConstants.WHITE_GROUND_COLOR;

    public WhitePlasticineSprite()
    {
        Color = PlasticineConstants.WHITE_GROUND_COLOR;
    }
}
