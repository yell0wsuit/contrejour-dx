using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class VariableSnotProcessor(LevelBuilderBase _builder) : SnotProcessor(_builder, "variableSnot")
{
    public override RopeMetricsWithCoords GetRopeMetricsEndItem(Vector2 startPoint, Vector2 endPoint, Hashtable item)
    {
        float num = item.GetHashtable("config").GetFloat("variableSize") * builder.SizeMult;
        return RopeUtil.GetRopeMetricsEndMaxPartSizeMinPartsLength(length: startPoint.DistanceTo(endPoint) + num, start: startPoint, end: endPoint, maxPartSize: partSize, minParts: 3);
    }
}
