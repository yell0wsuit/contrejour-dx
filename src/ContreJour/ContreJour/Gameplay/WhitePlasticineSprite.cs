using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay;

public class WhitePlasticineSprite : PlasticineSprite
{
    public override Color Color => PlasticineConstants.WhiteGroundColor;

    public WhitePlasticineSprite()
    {
        Color = PlasticineConstants.WhiteGroundColor;
    }
}
