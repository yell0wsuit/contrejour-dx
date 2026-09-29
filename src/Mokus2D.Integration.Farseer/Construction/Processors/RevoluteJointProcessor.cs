using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction.Processors
{
    public class RevoluteJointProcessor(PhysicsConstructor constructor) : JointProcessor(constructor)
    {
        public override Joint Process(Node item)
        {
            Vector2 anchor = Constructor.ToPhysics(item);
            RevoluteJoint revoluteJoint = new(GetBodyA(item), GetBodyB(item), anchor, useWorldCoordinates: true);
            if (item.Config.GetBool("limitEnabled"))
            {
                revoluteJoint.LimitEnabled = true;
                revoluteJoint.LowerLimit = XnaMath.ToRadians(item.Config.GetFloat("lowerLimit"));
                revoluteJoint.UpperLimit = XnaMath.ToRadians(item.Config.GetFloat("upperLimit"));
            }
            revoluteJoint.CollideConnected = item.Config.GetBool("collideConnected");
            return revoluteJoint;
        }
    }
}
