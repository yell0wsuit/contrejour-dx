using System.Collections.Generic;

using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction.Processors;

public class RevoluteJointProcessor : JointProcessor
{
    private const string LimitEnabled = "limitEnabled";

    public RevoluteJointProcessor(PhysicsConstructor constructor)
        : base(constructor)
    {
    }

    public override Joint Process(Node item)
    {
        Vector2 anchor = Constructor.ToPhysics(item);
        RevoluteJoint revoluteJoint = new RevoluteJoint(GetBodyA(item), GetBodyB(item), anchor, useWorldCoordinates: true);
        if (item.Config.GetBool("limitEnabled"))
        {
            revoluteJoint.LimitEnabled = true;
            revoluteJoint.LowerLimit = item.Config.GetFloat("lowerLimit").ToRadians();
            revoluteJoint.UpperLimit = item.Config.GetFloat("upperLimit").ToRadians();
        }
        revoluteJoint.CollideConnected = item.Config.GetBool("collideConnected");
        return revoluteJoint;
    }
}
