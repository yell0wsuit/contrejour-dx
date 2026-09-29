using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Factories;

public static class JointFactory
{
    public static MotorJoint CreateMotorJoint(World world, Body bodyA, Body bodyB, bool useWorldCoordinates = false)
    {
        MotorJoint motorJoint = new(bodyA, bodyB, useWorldCoordinates);
        world.AddJoint(motorJoint);
        return motorJoint;
    }

    public static RevoluteJoint CreateRevoluteJoint(World world, Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, bool useWorldCoordinates = false)
    {
        RevoluteJoint revoluteJoint = new(bodyA, bodyB, anchorA, anchorB, useWorldCoordinates);
        world.AddJoint(revoluteJoint);
        return revoluteJoint;
    }

    public static RevoluteJoint CreateRevoluteJoint(World world, Body bodyA, Body bodyB, Vector2 anchor)
    {
        Vector2 localPoint = bodyA.GetLocalPoint(bodyB.GetWorldPoint(anchor));
        RevoluteJoint revoluteJoint = new(bodyA, bodyB, localPoint, anchor);
        world.AddJoint(revoluteJoint);
        return revoluteJoint;
    }

    public static RopeJoint CreateRopeJoint(World world, Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, bool useWorldCoordinates = false)
    {
        RopeJoint ropeJoint = new(bodyA, bodyB, anchorA, anchorB, useWorldCoordinates);
        world.AddJoint(ropeJoint);
        return ropeJoint;
    }

    public static WeldJoint CreateWeldJoint(World world, Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, bool useWorldCoordinates = false)
    {
        WeldJoint weldJoint = new(bodyA, bodyB, anchorA, anchorB, useWorldCoordinates);
        world.AddJoint(weldJoint);
        return weldJoint;
    }

    public static PrismaticJoint CreatePrismaticJoint(World world, Body bodyA, Body bodyB, Vector2 anchor, Vector2 axis, bool useWorldCoordinates = false)
    {
        PrismaticJoint prismaticJoint = new(bodyA, bodyB, anchor, axis, useWorldCoordinates);
        world.AddJoint(prismaticJoint);
        return prismaticJoint;
    }

    public static WheelJoint CreateWheelJoint(World world, Body bodyA, Body bodyB, Vector2 anchor, Vector2 axis, bool useWorldCoordinates = false)
    {
        WheelJoint wheelJoint = new(bodyA, bodyB, anchor, axis, useWorldCoordinates);
        world.AddJoint(wheelJoint);
        return wheelJoint;
    }

    public static WheelJoint CreateWheelJoint(World world, Body bodyA, Body bodyB, Vector2 axis)
    {
        return CreateWheelJoint(world, bodyA, bodyB, Vector2.Zero, axis);
    }

    public static AngleJoint CreateAngleJoint(World world, Body bodyA, Body bodyB)
    {
        AngleJoint angleJoint = new(bodyA, bodyB);
        world.AddJoint(angleJoint);
        return angleJoint;
    }

    public static DistanceJoint CreateDistanceJoint(World world, Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, bool useWorldCoordinates = false)
    {
        DistanceJoint distanceJoint = new(bodyA, bodyB, anchorA, anchorB, useWorldCoordinates);
        world.AddJoint(distanceJoint);
        return distanceJoint;
    }

    public static DistanceJoint CreateDistanceJoint(World world, Body bodyA, Body bodyB)
    {
        return CreateDistanceJoint(world, bodyA, bodyB, Vector2.Zero, Vector2.Zero);
    }

    public static FrictionJoint CreateFrictionJoint(World world, Body bodyA, Body bodyB, Vector2 anchor, bool useWorldCoordinates = false)
    {
        FrictionJoint frictionJoint = new(bodyA, bodyB, anchor, useWorldCoordinates);
        world.AddJoint(frictionJoint);
        return frictionJoint;
    }

    public static FrictionJoint CreateFrictionJoint(World world, Body bodyA, Body bodyB)
    {
        return CreateFrictionJoint(world, bodyA, bodyB, Vector2.Zero);
    }

    public static GearJoint CreateGearJoint(World world, Body bodyA, Body bodyB, Joint jointA, Joint jointB, float ratio)
    {
        GearJoint gearJoint = new(bodyA, bodyB, jointA, jointB, ratio);
        world.AddJoint(gearJoint);
        return gearJoint;
    }

    public static PulleyJoint CreatePulleyJoint(World world, Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, Vector2 worldAnchorA, Vector2 worldAnchorB, float ratio, bool useWorldCoordinates = false)
    {
        PulleyJoint pulleyJoint = new(bodyA, bodyB, anchorA, anchorB, worldAnchorA, worldAnchorB, ratio, useWorldCoordinates);
        world.AddJoint(pulleyJoint);
        return pulleyJoint;
    }

    public static FixedMouseJoint CreateFixedMouseJoint(World world, Body body, Vector2 worldAnchor)
    {
        FixedMouseJoint fixedMouseJoint = new(body, worldAnchor);
        world.AddJoint(fixedMouseJoint);
        return fixedMouseJoint;
    }
}
