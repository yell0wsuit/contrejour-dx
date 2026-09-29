using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class SpringSnotSprite : SnotSprite
{
    protected float activeProgress;

    private float previousActiveProgress = 1f;

    // Snot sprites are created by reflection with (game, snot, startWidth, centerWidth, endWidth);
    // this one has no use for the game.
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Part of the reflection constructor signature.")]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "An unread primary constructor parameter is a compiler warning.")]
    public SpringSnotSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth)
        : base(snot, startWidth, centerWidth, endWidth)
    {
    }

    public bool Active { get; set; }

    public override void Update(float time)
    {
        base.Update(time);
        activeProgress = Maths.StepTo(activeProgress, Active ? 1 : 0, 0.05f);
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
