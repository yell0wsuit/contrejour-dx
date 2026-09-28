using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class WhiteTrampolineSprite : BlackTrampolineSprite
{
    public WhiteTrampolineSprite(ContreJourGame _game, SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth)
        : base(_game, _snot, _startWidth, _centerWidth, _endWidth)
    {
    }

    public override Color MiddleColor()
    {
        return ContreJourConstants.WHITE_SNOT_END_COLOR;
    }

    public override Color StartColor()
    {
        return ContreJourConstants.WHITE_SNOT_START_COLOR;
    }
}
