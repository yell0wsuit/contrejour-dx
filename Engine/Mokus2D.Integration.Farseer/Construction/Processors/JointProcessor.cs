using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction.Processors;

public abstract class JointProcessor(PhysicsConstructor constructor) : PhysicsProcessor(constructor)
{
    public abstract Joint Process(Node item);

    protected Body GetBodyA(Node item)
    {
        return GetBody(item, "bodyA");
    }

    protected Body GetBodyB(Node item)
    {
        return GetBody(item, "bodyB");
    }

    private Body GetBody(Node item, string id)
    {
        string name = item.Config.GetString(id);
        return Constructor.GetCreatedBody(name);
    }
}
