using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class WhiteSnotSprite : BlackSnotSprite
{
    public WhiteSnotSprite(ContreJourGame _game, SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth)
        : base(_game, _snot, _startWidth, _centerWidth, _endWidth)
    {
    }

    public override Color initialStartColor()
    {
        return ContreJourConstants.WHITE_SNOT_START_COLOR;
    }

    public override Color initialEndColor()
    {
        return ContreJourConstants.WHITE_SNOT_END_COLOR;
    }

    public override Color EndColor()
    {
        Color result = Color.Lerp(BaseCircleColor(), EndCircleColor(), activeProgress);
        result.A = 0;
        return result;
    }
}
