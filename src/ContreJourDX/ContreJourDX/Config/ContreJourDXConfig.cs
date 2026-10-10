using System.Numerics;

namespace ContreJourDX.Config
{
    public static class ContreJourDXConfig
    {
        public static AspectRatio AspectRatio { get; set; }

        public static Vector2 RootSize { get; set; }

        public static readonly bool BackButtonVisible = true;

        public static Vector2 BackButtonPosition => RootSize - new Vector2(60f, 60f);
    }
}
