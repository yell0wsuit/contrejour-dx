using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class RevoluteJointDef
{
    public Body BodyA;

    public Body BodyB;

    public Vector2 LocalAnchorB;

    public RevoluteJointDef(RevoluteJoint joint)
    {
        BodyA = joint.BodyA;
        BodyB = joint.BodyB;
        LocalAnchorB = joint.LocalAnchorB;
    }

    public RevoluteJoint Create(World world)
    {
        return JointFactory.CreateRevoluteJoint(world, BodyA, BodyB, LocalAnchorB);
    }
}
