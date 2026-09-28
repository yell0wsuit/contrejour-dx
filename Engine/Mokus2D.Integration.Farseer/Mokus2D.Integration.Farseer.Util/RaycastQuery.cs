using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Util;

public class RaycastQuery
{
    private readonly List<Fixture> _fixtures = new List<Fixture>();

    public List<Fixture> Fixtures => _fixtures;

    public float ReportFixture(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
    {
        _fixtures.Add(fixture);
        return -1f;
    }
}
