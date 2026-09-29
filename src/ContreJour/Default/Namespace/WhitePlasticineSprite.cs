using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class WhitePlasticineSprite : PlasticineSprite
{
    public override Color Color => PlasticineConstants.WhiteGroundColor;

    public WhitePlasticineSprite()
    {
        Color = PlasticineConstants.WhiteGroundColor;
    }
}
