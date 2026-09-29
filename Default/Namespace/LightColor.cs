using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class LightColor
{
    public Color LightInColor { get; set; }

    public Color LightOutColor;

    public Color LightBorderColor { get; set; }

    public LightColor()
    {
    }

    public LightColor(Color lightInColor, Color lightOutColor)
    {
        LightInColor = lightInColor;
        LightOutColor = lightOutColor;
        LightBorderColor = lightOutColor.ChangeAlpha(0);
    }

    public LightColor Clone()
    {
        return (LightColor)MemberwiseClone();
    }
}
