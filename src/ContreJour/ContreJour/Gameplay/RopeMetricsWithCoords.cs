using System.Numerics;

namespace ContreJour.Gameplay
{
    public class RopeMetricsWithCoords : RopeMetrics
    {
        private Vector2 start;

        private Vector2 partOffset;

        public Vector2 PartOffset => partOffset;

        public RopeMetricsWithCoords(int parts, float partSize, Vector2 start, Vector2 end)
            : base(parts, partSize)
        {
            this.start = start;
            partOffset = end - start;
            partOffset *= 1f / parts;
        }

        public Vector2 GetPositionByIndex(int index)
        {
            return start + (partOffset * index);
        }
    }
}
