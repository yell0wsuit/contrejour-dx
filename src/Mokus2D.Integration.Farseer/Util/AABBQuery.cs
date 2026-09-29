using System.Collections.Generic;

using FarseerPhysics.Dynamics;

namespace Mokus2D.Integration.Farseer.Util
{
    public class AABBQuery(List<Fixture> fixtures)
    {
        public List<Fixture> Fixtures { get; } = fixtures;

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
}
