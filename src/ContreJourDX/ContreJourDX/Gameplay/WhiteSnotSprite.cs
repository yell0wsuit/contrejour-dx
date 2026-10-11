using Mokus2D.Graphics;

namespace ContreJourDX.Gameplay
{
    public class WhiteSnotSprite(ContreJourDXGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : BlackSnotSprite(game, snot, startWidth, centerWidth, endWidth)
    {
        public override Color InitialStartColor()
        {
            return ContreJourDXConstants.WhiteSnotStartColor;
        }

        public override Color InitialEndColor()
        {
            return ContreJourDXConstants.WhiteSnotEndColor;
        }

        public override Color EndColor()
        {
            Color result = Color.Lerp(BaseCircleColor(), EndCircleColor(), ActiveProgress);
            result.A = 0;
            return result;
        }
    }
}
