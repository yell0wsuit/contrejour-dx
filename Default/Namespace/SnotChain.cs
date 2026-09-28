namespace Default.Namespace;

public class SnotChain
{
    protected SnotBodyClip snot;

    protected float distance;

    protected float diff;

    public SnotBodyClip Snot => snot;

    public float Distance => distance;

    public float Diff
    {
        get => diff;
        set => diff = value;
    }

    public static object CreateWithSnotDistance(SnotBodyClip _snot, float _distance)
    {
        return new SnotChain(_snot, _distance);
    }

    public SnotChain(SnotBodyClip _snot, float _distance)
    {
        snot = _snot;
        distance = _distance;
    }
}
