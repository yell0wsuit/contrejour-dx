using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay
{
    public class WhiteTrampolineSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : BlackTrampolineSprite(game, snot, startWidth, centerWidth, endWidth)
    {
        public override Color MiddleColor()
        {
            return ContreJourConstants.WhiteSnotEndColor;
        }

        public override Color StartColor()
        {
            return ContreJourConstants.WhiteSnotStartColor;
        }
    }
}
