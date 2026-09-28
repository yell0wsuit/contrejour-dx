using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace Mokus2D.Integration.Farseer.Util;

public class RaycastQuery
{
    private readonly List<Fixture> _fixtures = [];

    public List<Fixture> Fixtures => _fixtures;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Signature of Farseer's RayCastCallback.")]
    public float ReportFixture(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
    {
        _fixtures.Add(fixture);
        return -1f;
    }
}
