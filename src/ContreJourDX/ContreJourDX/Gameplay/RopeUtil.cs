using System;
using System.Numerics;

namespace ContreJourDX.Gameplay
{
    public class RopeUtil
    {
        public static RopeMetrics GetRopeMetricsByLengthMaxPartSizeMinParts(float distance, float maxPartSize, int minParts)
        {
            int num = Math.Max((int)Math.Ceiling(distance / maxPartSize), minParts);
            return new RopeMetrics(num, distance / num);
        }

        public static RopeMetricsWithCoords GetRopeMetricsEndMaxPartSizeMinPartsLength(Vector2 start, Vector2 end, float maxPartSize, int minParts, float length)
        {
            RopeMetrics ropeMetricsByLengthMaxPartSizeMinParts = GetRopeMetricsByLengthMaxPartSizeMinParts(length, maxPartSize, minParts);
            return new RopeMetricsWithCoords(ropeMetricsByLengthMaxPartSizeMinParts.Parts, ropeMetricsByLengthMaxPartSizeMinParts.PartSize, start, end);
        }

        public static RopeMetricsWithCoords GetRopeMetricsEndMaxPartSizeMinParts(Vector2 start, Vector2 end, float maxPartSize, int minParts)
        {
            return GetRopeMetricsEndMaxPartSizeMinPartsLength(start, end, maxPartSize, minParts, (start - end).Length());
        }
    }
}
