using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Mokus2D.Integration.Farseer.Util;

internal struct FixtureDistanceComparer(Vector2 center) : IComparer<Fixture>
{
    private readonly Vector2 center = center;

    public int Compare(Fixture x, Fixture y)
    {
        float num = x.Body.Position.DistanceTo(center);
        float num2 = y.Body.Position.DistanceTo(center);
        if (num < num2)
        {
            return -1;
        }
        if (num == num2)
        {
            return 0;
        }
        return 2;
    }
}
