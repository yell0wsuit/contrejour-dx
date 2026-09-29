using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Controllers
{
    public abstract class Controller(ControllerType controllerType) : FilterData
    {
        public bool Enabled { get; set; }

        public World World { get; set; }

        private readonly ControllerType _type = controllerType;

        public override bool IsActiveOn(Body body)
        {
            return !body.ControllerFilter.IsControllerIgnored(_type) && base.IsActiveOn(body);
        }

        public abstract void Update(float dt);
    }
}
