using System;

namespace Mokus2D.Visual.Displacement.Magnets.Actions
{
    public class MagnetChangeDoubleRadius(DoubleCircleMagnet gridMagnet, float timeout, float targetRadius, float radiusDiff) : MagnetChangeRadius(gridMagnet, timeout, targetRadius)
    {
        private readonly float _radiusDiff = radiusDiff;

        private readonly DoubleCircleMagnet DoubleCircleMagnet = gridMagnet;

        protected override void UpdateMagnet(float ratio)
        {
            base.UpdateMagnet(ratio);
            float excludeRadius = Math.Max(0f, DoubleCircleMagnet.Radius - _radiusDiff);
            DoubleCircleMagnet.ExcludeRadius = excludeRadius;
        }
    }
}
