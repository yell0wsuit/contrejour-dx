using System.Collections.Generic;

using FarseerPhysics.Dynamics;

namespace Mokus2D.Integration.Farseer.Util;

public class AABBQuery
{
    public List<Fixture> Fixtures { get; }

    public AABBQuery(List<Fixture> fixtures)
    {
        Fixtures = fixtures;
    }

    public AABBQuery()
        : this([])
    {
    }

    public bool CallbackReportFixture(Fixture fixture)
    {
        Fixtures.Add(fixture);
        return true;
    }
}
