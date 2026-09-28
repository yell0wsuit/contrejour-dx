namespace FarseerPhysics.Common.PhysicsLogic;

public struct PhysicsLogicFilter
{
    private PhysicsLogicType ControllerIgnores;

    public void IgnorePhysicsLogic(PhysicsLogicType type)
    {
        ControllerIgnores |= type;
    }

    public void RestorePhysicsLogic(PhysicsLogicType type)
    {
        ControllerIgnores &= ~type;
    }

    public readonly bool IsPhysicsLogicIgnored(PhysicsLogicType type)
    {
        return (ControllerIgnores & type) == type;
    }
}
