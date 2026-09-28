using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Controllers;

public abstract class Controller : FilterData
{
    public bool Enabled;

    public World World;

    private ControllerType _type;

    public Controller(ControllerType controllerType)
    {
        _type = controllerType;
    }

    public override bool IsActiveOn(Body body)
    {
        if (body.ControllerFilter.IsControllerIgnored(_type))
        {
            return false;
        }
        return base.IsActiveOn(body);
    }

    public abstract void Update(float dt);
}
