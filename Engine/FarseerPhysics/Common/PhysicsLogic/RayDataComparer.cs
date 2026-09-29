using System.Collections.Generic;

namespace FarseerPhysics.Common.PhysicsLogic;

internal sealed class RayDataComparer : IComparer<float>
{
    int IComparer<float>.Compare(float a, float b)
    {
        float num = a - b;
        return num > 0f ? 1 : num < 0f ? -1 : 0;
    }
}
