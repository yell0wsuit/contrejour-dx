using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class SpringSnotSprite(ContreJourGame _game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : SnotSprite(snot, startWidth, centerWidth, endWidth)
{
    private ContreJourGame game = _game;

    private bool active;

    protected float activeProgress;

    private float previousActiveProgress = 1f;

    public bool Active
    {
        get => active;
        set => active = value;
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
