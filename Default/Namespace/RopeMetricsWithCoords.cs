using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class RopeMetricsWithCoords : RopeMetrics
{
    protected Vector2 start;

    protected Vector2 end;

    protected Vector2 partOffset;

    public Vector2 PartOffset => partOffset;

    public RopeMetricsWithCoords(int _parts, float _partSize, Vector2 _start, Vector2 _end)
        : base(_parts, _partSize)
    {
        start = _start;
        end = _end;
        partOffset = _end - _start;
        partOffset *= 1f / _parts;
    }

    public Vector2 GetPositionByIndex(int index)
    {
        return start + (partOffset * index);
    }
}
