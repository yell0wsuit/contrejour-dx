using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Common.PhysicsLogic;

public abstract class PhysicsLogic : FilterData
{
	private PhysicsLogicType _type;

	public World World;

	public override bool IsActiveOn(Body body)
	{
		if (body.PhysicsLogicFilter.IsPhysicsLogicIgnored(_type))
		{
			return false;
		}
		return base.IsActiveOn(body);
	}

	public PhysicsLogic(World world, PhysicsLogicType type)
	{
		_type = type;
		World = world;
	}
}
