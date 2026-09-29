using System;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Util.Extensions
{
    public static class RandomExtensions
    {
        public static float RandomOffset(this Random source, float center, float maxOffset)
        {
            return center + source.Range(0f - maxOffset, maxOffset);
        }
    }
}
