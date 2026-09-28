using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class SpringSnotSprite : SnotSprite
{
    protected ContreJourGame game;

    protected bool active;

    protected float activeProgress;

    protected float previousActiveProgress;

    public bool Active
    {
        get => active;
        set => active = value;
    }

    public SpringSnotSprite(ContreJourGame _game, SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth)
        : base(_snot, _startWidth, _centerWidth, _endWidth)
    {
        game = _game;
        activeProgress = 0f;
        previousActiveProgress = 1f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        activeProgress = Maths.StepTo(activeProgress, active ? 1 : 0, 0.05f);
        if (Maths.FuzzyNotEquals(activeProgress, previousActiveProgress))
        {
            SetCirclesColors();
            previousActiveProgress = activeProgress;
        }
        _ = snot.StartPosition;
        _ = snot.EndPosition();
    }

    public virtual Color BaseCircleColor()
    {
        return Color;
    }

    public virtual Color EndCircleColor()
    {
        return Color;
    }

    public virtual void DrawCircles()
    {
    }

    public static void SetCirclesColors()
    {
    }

    public new virtual Color EndColor()
    {
        int num = (int)(200f * activeProgress);
        return new Color(num, num, num, 0);
    }
}
