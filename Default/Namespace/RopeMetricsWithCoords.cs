using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class RopeMetricsWithCoords : RopeMetrics
{
    protected Vector2 start;

    protected Vector2 end;

    protected Vector2 partOffset;

    public Vector2 PartOffset => partOffset;

    public RopeMetricsWithCoords(int parts, float partSize, Vector2 start, Vector2 end)
        : base(parts, partSize)
    {
        this.start = start;
        this.end = end;
        partOffset = end - start;
        partOffset *= 1f / parts;
    }

    public Vector2 GetPositionByIndex(int index)
    {
        return start + (partOffset * index);
    }
}
