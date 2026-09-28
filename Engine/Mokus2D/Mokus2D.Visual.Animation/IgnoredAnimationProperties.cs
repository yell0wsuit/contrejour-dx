namespace Mokus2D.Visual.Animation;

public struct IgnoredAnimationProperties(bool visible = false, bool opacity = false, bool color = false, bool position = false, bool rotation = false, bool scale = false)
{
    public static readonly IgnoredAnimationProperties None = default;

    public static readonly IgnoredAnimationProperties All = new(visible: true, opacity: true, color: true, position: true, rotation: true, scale: true);

    public bool Visible = visible;

    public bool Opacity = opacity;

    public bool Color = color;

    public bool Position = position;

    public bool Rotation = rotation;

    public bool Scale = scale;
}
