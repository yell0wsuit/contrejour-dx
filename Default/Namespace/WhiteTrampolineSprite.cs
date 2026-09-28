using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class WhiteTrampolineSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : BlackTrampolineSprite(game, snot, startWidth, centerWidth, endWidth)
{
    public override Color MiddleColor()
    {
        return ContreJourConstants.WHITE_SNOT_END_COLOR;
    }

    public override Color StartColor()
    {
        return ContreJourConstants.WHITE_SNOT_START_COLOR;
    }
}
