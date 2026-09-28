using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class VariableSnotProcessor : SnotProcessor
{
    public VariableSnotProcessor(LevelBuilderBase _builder)
        : base(_builder, "variableSnot")
    {
    }

    public override RopeMetricsWithCoords GetRopeMetricsEndItem(Vector2 start, Vector2 end, Hashtable item)
    {
        float num = item.GetHashtable("config").GetFloat("variableSize") * builder.SizeMult;
        return RopeUtil.GetRopeMetricsEndMaxPartSizeMinPartsLength(length: start.DistanceTo(end) + num, start: start, end: end, maxPartSize: partSize, minParts: 3);
    }
}
