namespace Default.Namespace;

public class SnotChain(SnotBodyClip _snot, float _distance)
{
    public SnotBodyClip Snot { get; } = _snot;

    public float Distance { get; } = _distance;

    public float Diff { get; set; }

    public static object CreateWithSnotDistance(SnotBodyClip snot, float distance)
    {
        return new SnotChain(snot, distance);
    }
}
