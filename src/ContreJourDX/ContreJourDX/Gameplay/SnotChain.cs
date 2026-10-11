namespace ContreJourDX.Gameplay
{
    public class SnotChain(SnotBodyClip snot, float distance)
    {
        public SnotBodyClip Snot { get; } = snot;

        public float Distance { get; } = distance;

        public float Diff { get; set; }

        public static object CreateWithSnotDistance(SnotBodyClip snot, float distance)
        {
            return new SnotChain(snot, distance);
        }
    }
}
