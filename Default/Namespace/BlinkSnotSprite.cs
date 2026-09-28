using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class BlinkSnotSprite : SpringSnotSprite
{
    protected bool highlite;

    protected float highliteStep;

    protected Color initialColor;

    private static readonly Color HIGHLITE_COLOR = ContreJourConstants.BLUE_LIGHT_COLOR;

    public bool Highlite
    {
        get
        {
            return highlite;
        }
        set
        {
            highlite = value;
        }
    }

    private BlinkSnotSprite(ContreJourGame _game, SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth)
        : base(_game, _snot, _startWidth, _centerWidth, _endWidth)
    {
        highliteStep = 0f;
        initialColor = Color;
        highlite = true;
    }

    public override void Update(float time)
    {
        base.Update(time);
        float a = highliteStep;
        highliteStep = Maths.StepTo(highliteStep, highlite ? 1 : 0, 0.02f);
        if (Maths.FuzzyNotEquals(a, highliteStep))
        {
            Color = Color.Lerp(initialColor, HIGHLITE_COLOR, highliteStep).ChangeAlpha(Color.A);
            SetCirclesColors();
        }
    }
}
