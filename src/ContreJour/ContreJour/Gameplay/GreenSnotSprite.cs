using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay
{
    public class GreenSnotSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : BlackSnotSprite(game, snot, startWidth, centerWidth, endWidth)
    {
        public override Color InitialStartColor()
        {
            return ContreJourConstants.GreenSnotStart;
        }

        public override Color InitialEndColor()
        {
            return ContreJourConstants.GreenSnotEnd;
        }

        public override Color EndColor()
        {
            Color result = Color.Lerp(BaseCircleColor(), EndCircleColor(), ActiveProgress);
            result.A = 0;
            return result;
        }
    }
}
