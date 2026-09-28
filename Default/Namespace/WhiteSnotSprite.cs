using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class WhiteSnotSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : BlackSnotSprite(game, snot, startWidth, centerWidth, endWidth)
{
    public override Color InitialStartColor()
    {
        return ContreJourConstants.WhiteSnotStartColor;
    }

    public override Color InitialEndColor()
    {
        return ContreJourConstants.WhiteSnotEndColor;
    }

    public override Color EndColor()
    {
        Color result = Color.Lerp(BaseCircleColor(), EndCircleColor(), activeProgress);
        result.A = 0;
        return result;
    }
}
