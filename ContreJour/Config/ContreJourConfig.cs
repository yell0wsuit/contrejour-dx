using Microsoft.Xna.Framework;

namespace ContreJour.Config;

public static class ContreJourConfig
{
    public static AspectRatio AspectRatio { get; set; }

    public static Vector2 RootSize { get; set; }

    public static readonly bool BackButtonVisible = true;

    public static Vector2 BackButtonPosition => RootSize - new Vector2(60f, 60f);
}
