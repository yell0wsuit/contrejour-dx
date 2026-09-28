namespace Default.Namespace;

public class SnotChain(SnotBodyClip _snot, float _distance)
{
    private SnotBodyClip snot = _snot;

    private float distance = _distance;

    private float diff;

    public SnotBodyClip Snot => snot;

    public float Distance => distance;

    public float Diff
    {
        get => diff;
        set => diff = value;
    }

    public static object CreateWithSnotDistance(SnotBodyClip snot, float distance)
    {
        return new SnotChain(snot, distance);
    }
}
