using Microsoft.Xna.Framework;

namespace ContreJour.Config;

public static class ContreJourConfig
{
    public static AspectRatio AspectRatio;

    public static Vector2 RootSize;

    public static bool BackButtonVisible = true;

    public static Vector2 BackButtonPosition => RootSize - new Vector2(60f, 60f);
}
