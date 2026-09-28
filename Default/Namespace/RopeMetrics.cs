namespace Default.Namespace;

public class RopeMetrics(int _parts, float _partSize)
{
    private int parts = _parts;

    private float partSize = _partSize;

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
}
