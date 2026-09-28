using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class RevoluteJointDef(RevoluteJoint joint)
{
    public Body BodyA = joint.BodyA;

    public Body BodyB = joint.BodyB;

    public Vector2 LocalAnchorB = joint.LocalAnchorB;

    public RevoluteJoint Create(World world)
    {
        return JointFactory.CreateRevoluteJoint(world, BodyA, BodyB, LocalAnchorB);
    }
}
