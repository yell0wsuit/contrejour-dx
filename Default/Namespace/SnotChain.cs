namespace Default.Namespace;

public class SnotChain(SnotBodyClip _snot, float _distance)
{
    protected SnotBodyClip snot = _snot;

    protected float distance = _distance;

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
}
