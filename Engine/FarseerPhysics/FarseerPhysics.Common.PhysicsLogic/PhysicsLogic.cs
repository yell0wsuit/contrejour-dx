using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Common.PhysicsLogic;

public abstract class PhysicsLogic : FilterData
{
    private PhysicsLogicType _type;

    public World World;

    public override bool IsActiveOn(Body body)
    {
        return !body.PhysicsLogicFilter.IsPhysicsLogicIgnored(_type) && base.IsActiveOn(body);
    }

    public PhysicsLogic(World world, PhysicsLogicType type)
    {
        _type = type;
        World = world;
    }
}
