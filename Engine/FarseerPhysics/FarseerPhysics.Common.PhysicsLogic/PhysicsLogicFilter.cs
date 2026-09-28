namespace FarseerPhysics.Common.PhysicsLogic;

public struct PhysicsLogicFilter
{
	public PhysicsLogicType ControllerIgnores;

	public void IgnorePhysicsLogic(PhysicsLogicType type)
	{
		ControllerIgnores |= type;
	}

	public void RestorePhysicsLogic(PhysicsLogicType type)
	{
		ControllerIgnores &= ~type;
	}

	public bool IsPhysicsLogicIgnored(PhysicsLogicType type)
	{
		return (ControllerIgnores & type) == type;
	}
}
