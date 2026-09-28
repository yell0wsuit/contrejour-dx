namespace Default.Namespace;

public class RopeMetrics
{
    protected int parts;

    protected float partSize;

    public int Parts
    {
        get => parts;
        set => parts = value;
    }

    public float PartSize
    {
        get => partSize;
        set => partSize = value;
    }

    public RopeMetrics(int _parts, float _partSize)
    {
        parts = _parts;
        partSize = _partSize;
    }
}
