using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay
{
    public class VariableSnotProcessor(LevelBuilderBase builder) : SnotProcessor(builder, "variableSnot")
    {
        public override RopeMetricsWithCoords GetRopeMetricsEndItem(Vector2 startPoint, Vector2 endPoint, Hashtable item)
        {
            float num = item.GetHashtable("config").GetFloat("variableSize") * Builder.SizeMult;
            return RopeUtil.GetRopeMetricsEndMaxPartSizeMinPartsLength(length: Vector2.Distance(startPoint, endPoint) + num, start: startPoint, end: endPoint, maxPartSize: MaxPartSize, minParts: 3);
        }
    }
}
