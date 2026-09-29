using System;
using System.Collections.Generic;
using System.Reflection;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

using Mokus2D.Integration.Farseer.Config;
using Mokus2D.Integration.Farseer.Physics;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Integration.Farseer.Util;

public static class FarseerUtil
{
    public delegate bool ClipPredicate(BodyClip clip, object param);


    public static bool DynamicBodyPredicate(Body body)
    {
        return body.BodyType == BodyType.Dynamic;
    }

    public static bool StaticBodyPredicate(Body body)
    {
        return body.BodyType == BodyType.Static;
    }

    public static bool DynamicObjectPredicate(object objectP)
    {
        return ((Body)objectP).BodyType == BodyType.Dynamic;
    }

    public static bool KinematicObjectPredicate(object objectP)
    {
        return ((Body)objectP).BodyType == BodyType.Kinematic;
    }

    private static bool InterfacePredicate(object clip, object interfaceType)
    {
        return ((Type)interfaceType).GetTypeInfo().IsAssignableFrom(clip.GetType().GetTypeInfo());
    }

    private static bool TypePredicate(BodyClip clip, object type)
    {
        return ReflectionHelperExtensions.IsInstanceOfType((Type)type, clip);
    }

    public static void LimitSpeed(Body body, float maxSpeed)
    {
        Vector2 linearVelocity = body.LinearVelocity;
        float num = linearVelocity.Length();
        if (num > maxSpeed)
        {
            linearVelocity *= maxSpeed / num;
            body.LinearVelocity = linearVelocity;
        }
    }

    public static void SetDensity(this Body body, float value)
    {
        foreach (Fixture fixture in body.FixtureList)
        {
            fixture.Shape.Density = value;
        }
        body.ResetMassData();
    }

    public static void SetGroupIndex(this Body body, short index)
    {
        foreach (Fixture fixture in body.FixtureList)
        {
            fixture.CollisionGroup = index;
        }
    }

    public static RevoluteJoint CreateRevoluteJoint(World world, Body body1, Body body2, Vector2 position, bool collideConnected = false, bool limitAngles = false)
    {
        RevoluteJoint revoluteJoint = JointFactory.CreateRevoluteJoint(world, body1, body2, position - body2.Position);
        revoluteJoint.CollideConnected = collideConnected;
        if (limitAngles)
        {
            revoluteJoint.LowerLimit = 0f;
            revoluteJoint.UpperLimit = 0f;
            revoluteJoint.LimitEnabled = true;
        }
        return revoluteJoint;
    }

    public static DistanceJoint CreateDistanceJoint(World world, Body body1, Body body2, float freq, float damping, bool collideConnected = false)
    {
        DistanceJoint distanceJoint = JointFactory.CreateDistanceJoint(world, body1, body2, Vector2.Zero, Vector2.Zero);
        distanceJoint.CollideConnected = collideConnected;
        distanceJoint.Frequency = freq;
        distanceJoint.DampingRatio = damping;
        return distanceJoint;
    }

    public static Vector2 GetWorldPoint(this Contact contact)
    {
        contact.GetWorldManifold(out Vector2 _, out FixedArray2<Vector2> points);
        return points[0];
    }

    public static bool IsTouching(this Body body, Type type)
    {
        for (ContactEdge contactEdge = body.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
        {
            if (contactEdge.Contact.IsTouching && !contactEdge.Contact.FixtureA.IsSensor && !contactEdge.Contact.FixtureB.IsSensor && ((object)type == typeof(Nullable) || (contactEdge.Other.UserData != null && ReflectionHelperExtensions.IsInstanceOfType(type, contactEdge.Other.UserData))))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsTouching(this Body bodyA, Body bodyB)
    {
        for (ContactEdge contactEdge = bodyA.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
        {
            if (contactEdge.Contact.IsTouching && contactEdge.Other == bodyB)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsTouching(Body body)
    {
        return body.IsTouching(typeof(Nullable));
    }

    public static bool IsSensor(this Contact contact)
    {
        return contact.FixtureA.IsSensor || contact.FixtureB.IsSensor;
    }

    public static bool IsSensor(this Body body)
    {
        foreach (Fixture fixture in body.FixtureList)
        {
            if (!fixture.IsSensor)
            {
                return false;
            }
        }
        return true;
    }

    public static Body CreateCircle(this World world, float radius, Vector2 position, float rotation = 0f, float density = 0f, bool dynamic = false)
    {
        CircleShape shape = new(radius, density);
        return BodyFromShape(world, shape, position, rotation, sensor: false, density, dynamic);
    }

    public static Body CreateBox(World world, Vector2 position, List<Vector2> vertices, bool sensor, float density, bool dynamic)
    {
        Vertices vertices2 = [.. vertices];
        PolygonShape shape = new(vertices2, density);
        return BodyFromShape(world, shape, position, 0f, sensor, density, dynamic);
    }

    public static Body CreateBox(World world, Vector2 position, float width, float height, float rotation = 0f, bool sensor = false, float density = 1f, bool dynamic = false)
    {
        PolygonShape shape = new(density);
        shape.SetAsBox(width / 2f, height / 2f);
        return BodyFromShape(world, shape, position, rotation, sensor, density, dynamic);
    }

    public static Fixture AddShape(Shape shape, Body body, float density)
    {
        Fixture fixture = body.CreateFixture(shape, density);
        fixture.Friction = FarseerConfig.DefaultConfig.Friction;
        return fixture;
    }

    public static Body BodyFromShape(World world, Shape shape, Vector2 position, float rotation, bool sensor, float density, bool dynamic)
    {
        Body body = BodyFactory.CreateBody(world);
        body.BodyType = dynamic ? BodyType.Dynamic : BodyType.Static;
        body.Position = position;
        body.Rotation = rotation;
        Fixture fixture = AddShape(shape, body, density);
        fixture.Friction = FarseerConfig.DefaultConfig.Friction;
        fixture.IsSensor = sensor;
        return body;
    }

    public static List<Fixture> Raycast(this World world, Vector2 startPoint, Vector2 endPoint)
    {
        RaycastQuery raycastQuery = new();
        world.RayCast(raycastQuery.ReportFixture, startPoint, endPoint);
        return raycastQuery.Fixtures;
    }

    public static List<Fixture> Query(this World world, Vector2 point)
    {
        List<Fixture> list = [];
        AABB aabb = CreateOnePixelAabb(point);
        AABBQuery aABBQuery = new();
        world.QueryAABB(aABBQuery.CallbackReportFixture, ref aabb);
        for (int i = 0; i < aABBQuery.Fixtures.Count; i++)
        {
            Fixture fixture = aABBQuery.Fixtures[i];
            if (fixture.TestPoint(ref point))
            {
                list.Add(fixture);
            }
        }
        return list;
    }

    public static void MoveWithLimit(this Body body, Vector2 position, float teleportCoeff, float time, float maxDistance)
    {
        position = position.ClampDistance(body.Position, maxDistance);
        body.Move(position, teleportCoeff, time);
    }

    public static void Move(this Body body, Vector2 position, float teleportCoeff, float time)
    {
        body.Move(position, body.Rotation, teleportCoeff, time);
    }

    public static void Move(this Body body, Vector2 position, float angle, float teleportCoeff, float time)
    {
        if (time <= 0f)
        {
            teleportCoeff = 1f;
        }
        float num = 1f - teleportCoeff;
        Vector2 vector = position - body.Position;
        float num2 = angle - body.Rotation;
        if (num > 0f)
        {
            Vector2 linearVelocity = vector;
            linearVelocity *= num / time;
            vector *= teleportCoeff;
            body.LinearVelocity = linearVelocity;
            body.AngularVelocity = num2 * num / time;
        }
        body.SetTransform(body.Position + vector, body.Rotation + (num2 * teleportCoeff));
    }

    public static BodyClip Query(World world, Vector2 center, float radius, Type type)
    {
        List<Fixture> list = world.Query(center, radius);
        foreach (Fixture item in list)
        {
            if (item.Body.UserData != null && ReflectionHelperExtensions.IsInstanceOfType(type, item.Body.UserData))
            {
                return (BodyClip)item.Body.UserData;
            }
        }
        return null;
    }

    public static List<BodyClip> QueryClips(World world, Vector2 center, float radius, Type protocol)
    {
        return Query(world, center, radius, InterfacePredicate, protocol);
    }

    public static List<BodyClip> QueryBodyClipsCenterRadiusType(World world, Vector2 center, float radius, Type type)
    {
        return Query(world, center, radius, TypePredicate, type);
    }

    public static List<BodyClip> Query(World world, Vector2 center, float radius, ClipPredicate clipPredicate, object param)
    {
        List<Fixture> list = world.Query(center, radius);
        List<BodyClip> list2 = [];
        foreach (Fixture item in list)
        {
            if (item.Body.UserData is BodyClip bodyClip && clipPredicate(bodyClip, param))
            {
                list2.Add(bodyClip);
            }
        }
        return list2;
    }

    private static List<Fixture> Query(this World world, Vector2 center, float radius)
    {
        AABB aabb = CreateAABB(center, radius * 2f, radius * 2f);
        return world.Query(ref aabb);
    }

    public static Fixture GetClosestFixture(this World world, Vector2 center, float width, float height)
    {
        List<Fixture> source = world.Query(center, width, height);
        return source.Min(new FixtureDistanceComparer(center));
    }

    public static List<Fixture> Query(this World world, Vector2 center, float width, float height)
    {
        AABB aabb = CreateAABB(center, width, height);
        return world.Query(ref aabb);
    }

    public static void Query(this World world, AABBQuery result, Vector2 center, float width, float height)
    {
        AABB aabb = CreateAABB(center, width, height);
        world.QueryAABB(result.CallbackReportFixture, ref aabb);
    }

    public static List<Fixture> Query(this World world, ref AABB aabb)
    {
        AABBQuery aABBQuery = new();
        world.QueryAABB(aABBQuery.CallbackReportFixture, ref aabb);
        return aABBQuery.Fixtures;
    }

    public static void FixturesUnderPoint(List<Fixture> fixtures, World world, Vector2 point)
    {
        foreach (Body body in world.BodyList)
        {
            foreach (Fixture fixture in body.FixtureList)
            {
                if (fixture.TestPoint(ref point))
                {
                    fixtures.Add(fixture);
                }
            }
        }
    }

    private static AABB CreateOnePixelAabb(Vector2 center)
    {
        float num = 1f / FarseerConfig.DefaultConfig.PhysicsToPixels;
        return CreateAABB(center, num, num);
    }

    public static AABB CreateAABB(Vector2 center, float width, float height)
    {
        AABB result = new()
        {
            LowerBound = center,
            UpperBound = center
        };
        result.LowerBound -= new Vector2(width / 2f, height / 2f);
        result.UpperBound += new Vector2(width / 2f, height / 2f);
        return result;
    }

    public static void CreateBorder(Body groundBody, Vector2 start, Vector2 end, Vector2 direction)
    {
        for (int i = 0; i < 2; i++)
        {
            _ = FixtureFactory.AttachEdge(start, end, groundBody);
            start += direction;
            end += direction;
        }
    }
}
