using System.Collections.Generic;
using FarseerPhysics.Dynamics;

namespace Mokus2D.Integration.Farseer.Util;

public class AABBQuery
{
	private readonly List<Fixture> _fixtures;

	public List<Fixture> Fixtures => _fixtures;

	public AABBQuery(List<Fixture> fixtures)
	{
		_fixtures = fixtures;
	}

	public AABBQuery()
		: this(new List<Fixture>())
	{
	}

	public bool CallbackReportFixture(Fixture fixture)
	{
		_fixtures.Add(fixture);
		return true;
	}
}
