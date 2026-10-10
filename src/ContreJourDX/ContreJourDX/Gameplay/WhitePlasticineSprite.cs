using Mokus2D.Graphics;

namespace ContreJourDX.Gameplay
{
    public class WhitePlasticineSprite : PlasticineSprite
    {
        public override Color Color => PlasticineConstants.WhiteGroundColor;

        public WhitePlasticineSprite()
        {
            Color = PlasticineConstants.WhiteGroundColor;
        }
    }
}
