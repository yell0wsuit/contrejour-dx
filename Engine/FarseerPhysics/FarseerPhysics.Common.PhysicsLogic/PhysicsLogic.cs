using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Common.PhysicsLogic;

public abstract class PhysicsLogic(World world, PhysicsLogicType type) : FilterData
{
    private readonly PhysicsLogicType _type = type;

    public World World = world;

    public override bool IsActiveOn(Body body)
    {
        return !body.PhysicsLogicFilter.IsPhysicsLogicIgnored(_type) && base.IsActiveOn(body);
    }
}
