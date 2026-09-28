using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class RevoluteJointDef(RevoluteJoint joint)
{
    private Body BodyA = joint.BodyA;

    private Body BodyB = joint.BodyB;

    private Vector2 LocalAnchorB = joint.LocalAnchorB;

    public RevoluteJoint Create(World world)
    {
        return JointFactory.CreateRevoluteJoint(world, BodyA, BodyB, LocalAnchorB);
    }
}
