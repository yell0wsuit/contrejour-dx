using Mokus2D.Graphics;

namespace ContreJourDX.Gameplay
{
    public class WhiteTrampolineSprite(ContreJourDXGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : BlackTrampolineSprite(game, snot, startWidth, centerWidth, endWidth)
    {
        public override Color MiddleColor()
        {
            return ContreJourDXConstants.WhiteSnotEndColor;
        }

        public override Color StartColor()
        {
            return ContreJourDXConstants.WhiteSnotStartColor;
        }
    }
}
