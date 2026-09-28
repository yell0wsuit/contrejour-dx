using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Default.Namespace;

internal readonly struct FixtureDistanceComparer(Vector2 center) : IComparer<Fixture>
{
    private readonly Vector2 center = center;

    public readonly int Compare(Fixture x, Fixture y)
    {
        float num = x.Body.Position.DistanceTo(center);
        float num2 = y.Body.Position.DistanceTo(center);
        if (num < num2)
        {
            return -1;
        }
        return num == num2 ? 0 : 2;
    }
}
