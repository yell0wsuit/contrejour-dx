using Mokus2D.Graphics;

namespace ContreJourDX.Gameplay
{
    public class GreenSnotSprite(ContreJourDXGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : BlackSnotSprite(game, snot, startWidth, centerWidth, endWidth)
    {
        public override Color InitialStartColor()
        {
            return ContreJourDXConstants.GreenSnotStart;
        }

        public override Color InitialEndColor()
        {
            return ContreJourDXConstants.GreenSnotEnd;
        }

        public override Color EndColor()
        {
            Color result = Color.Lerp(BaseCircleColor(), EndCircleColor(), ActiveProgress);
            result.A = 0;
            return result;
        }
    }
}
