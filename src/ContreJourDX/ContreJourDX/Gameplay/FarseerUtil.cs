using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Mokus2D.Integration.Farseer.Util;
using Mokus2D.Util;

namespace ContreJourDX.Gameplay
{
    public static class FarseerUtil
    {
        public delegate bool ClipPredicate(BodyClip clip, object param);

        public struct Borders(bool left, bool top, bool right, bool bottom)
        {
            public bool Bottom = bottom;

            public bool Left = left;

            public bool Right = right;

            public bool Top = top;
        }

        private static readonly string IdString = "id";

        static FarseerUtil()
        {
        }

        public static bool DynamicBodyPredicate(Body body)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0007: Invalid comparison between Unknown and I4
            return (int)body.BodyType == 2;
        }

        public static bool StaticBodyPredicate(Body body)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0007: Invalid comparison between Unknown and I4
            return body.BodyType == 0;
        }

        public static bool DynamicObjectPredicate(object objectP)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Invalid comparison between Unknown and I4
            return (int)((Body)objectP).BodyType == 2;
        }

        public static bool KinematicObjectPredicate(object objectP)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Invalid comparison between Unknown and I4
            return (int)((Body)objectP).BodyType == 1;
        }

        private static bool InterfacePredicate(object clip, object interfaceType)
        {
            return ((Type)interfaceType).GetTypeInfo().IsAssignableFrom(clip.GetType().GetTypeInfo());
        }

        private static bool TypePredicate(BodyClip clip, object type)
        {
            return ReflectionHelperExtensions.IsInstanceOfType((Type)type, clip);
        }

        public static Fixture GetFixture(Body body, string property)
        {
            return GetFixture(body, property, "true");
        }

        public static Fixture GetFixture(Body body, string property, string value)
        {
            foreach (Fixture fixture in body.FixtureList)
            {
                if (fixture.UserData is Hashtable hashtable && hashtable.Exists(property) && value == hashtable.GetString(property))
                {
                    return fixture;
                }
            }
            return null;
        }

        public static void LimitSpeed(Body body, float maxSpeed)
        {
            Vector2 linearVelocity = body.LinearVelocity;
            float num = linearVelocity.Length();
            if (num > maxSpeed)
            {
                body.LinearVelocity = linearVelocity * (maxSpeed / num);
            }
        }

        public static Fixture FixtureById(string id, Body body)
        {
            return GetFixture(body, IdString, id);
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
            RevoluteJoint val = JointFactory.CreateRevoluteJoint(world, body1, body2, position - body2.Position);
            val.CollideConnected = collideConnected;
            if (limitAngles)
            {
                val.LowerLimit = 0f;
                val.UpperLimit = 0f;
                val.LimitEnabled = true;
            }
            return val;
        }

        public static DistanceJoint CreateDistanceJoint(World world, Body body1, Body body2, float freq, float damping, bool collideConnected = false)
        {
            DistanceJoint val = JointFactory.CreateDistanceJoint(world, body1, body2, Vector2.Zero, Vector2.Zero, false);
            val.CollideConnected = collideConnected;
            val.Frequency = freq;
            val.DampingRatio = damping;
            return val;
        }

        public static void SetSensor(this Body body, bool value)
        {
            foreach (Fixture fixture in body.FixtureList)
            {
                fixture.IsSensor = value;
            }
        }

        public static Vector2 GetWorldPoint(Contact contact)
        {
            contact.GetWorldManifold(out _, out FixedArray2<Vector2> val);
            return val[0];
        }

        public static bool IsTouching(this Body body, Type type)
        {
            for (ContactEdge val = body.ContactList; val != null; val = val.Next)
            {
                if (val.Contact.IsTouching && !val.Contact.FixtureA.IsSensor && !val.Contact.FixtureB.IsSensor && ((object)type == typeof(Nullable) || (val.Other.UserData != null && ReflectionHelperExtensions.IsInstanceOfType(type, val.Other.UserData))))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTouching(this Body bodyA, Body bodyB)
        {
            for (ContactEdge val = bodyA.ContactList; val != null; val = val.Next)
            {
                if (val.Contact.IsTouching && val.Other == bodyB)
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
            //IL_0003: Unknown result type (might be due to invalid IL or missing references)
            //IL_0009: Expected O, but got Unknown
            return BodyFromShape(world, new CircleShape(radius, density), position, rotation, sensor: false, density, dynamic);
        }

        public static Body CreateBox(World world, Vector2 position, List<Vector2> vertices, bool sensor, float density, bool dynamic)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0007: Expected O, but got Unknown
            //IL_000a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0010: Expected O, but got Unknown
            return BodyFromShape(world, new PolygonShape([.. vertices], density), position, 0f, sensor, density, dynamic);
        }

        public static Body CreateBox(World world, Vector2 position, float width, float height, float rotation, bool sensor, float density, bool dynamic)
        {
            //IL_0002: Unknown result type (might be due to invalid IL or missing references)
            //IL_0008: Expected O, but got Unknown
            PolygonShape shape = new(density);
            shape.SetAsBox(width / 2f, height / 2f);
            return BodyFromShape(world, (Shape)(object)shape, position, rotation, sensor, density, dynamic);
        }

        public static Fixture AddShape(Shape shape, Body body, float density)
        {
            Fixture val = body.CreateFixture(shape, density);
            val.Friction = Box2DConfig.DefaultConfig.Friction;
            return val;
        }

        public static Body BodyFromShape(World world, Shape shape, Vector2 position, float rotation, bool sensor, float density, bool dynamic)
        {
            Body val = BodyFactory.CreateBody(world, null);
            val.BodyType = (BodyType)(dynamic ? 2 : 0);
            val.Position = position;
            val.Rotation = rotation;
            Fixture val2 = AddShape(shape, val, density);
            val2.Friction = Box2DConfig.DefaultConfig.Friction;
            val2.IsSensor = sensor;
            return val;
        }

        public static List<Fixture> Raycast(this World world, Vector2 startPoint, Vector2 endPoint)
        {
            RaycastQuery raycastQuery = new();
            world.RayCast(raycastQuery.ReportFixture, startPoint, endPoint);
            return raycastQuery.Fixtures;
        }

        public static List<Fixture> Query(this World world, Vector2 point)
        {
            //IL_0007: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            List<Fixture> list = [];
            AABB val = CreateAabb(point);
            AABBQuery aABBQuery = new();
            world.QueryAABB(aABBQuery.CallbackReportFixture, ref val);
            for (int i = 0; i < aABBQuery.Fixtures.Count; i++)
            {
                Fixture val2 = aABBQuery.Fixtures[i];
                if (val2.TestPoint(ref point))
                {
                    list.Add(val2);
                }
            }
            return list;
        }

        public static void Move(this Body body, Vector2 position, float teleportCoeff, float time)
        {
            body.Move(position, body.Rotation, teleportCoeff, time);
        }

        public static void Move(this Body body, Vector2 position, float angle, float teleportCoeff, float time)
        {
            float num = 1f - teleportCoeff;
            Vector2 vector = position - body.Position;
            float num2 = angle - body.Rotation;
            Vector2 linearVelocity = vector;
            linearVelocity *= num / time;
            vector *= teleportCoeff;
            body.LinearVelocity = linearVelocity;
            body.AngularVelocity = num2 * num / time;
            body.SetTransform(body.Position + vector, body.Rotation + (num2 * teleportCoeff));
        }

        public static BodyClip Query(World world, Vector2 center, float radius, Type type)
        {
            foreach (Fixture item in world.Query(center, radius))
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
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_0016: Unknown result type (might be due to invalid IL or missing references)
            return world.Query(CreateAABB(center, radius * 2f, radius * 2f));
        }

        public static Fixture GetClosestFixture(this World world, Vector2 center, float width, float height)
        {
            return world.Query(center, width, height).Max(new FixtureDistanceComparer(center));
        }

        public static List<Fixture> Query(this World world, Vector2 center, float width, float height)
        {
            //IL_0003: Unknown result type (might be due to invalid IL or missing references)
            //IL_0008: Unknown result type (might be due to invalid IL or missing references)
            //IL_000a: Unknown result type (might be due to invalid IL or missing references)
            return world.Query(CreateAABB(center, width, height));
        }

        public static List<Fixture> Query(this World world, AABB aabb)
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

        private static AABB CreateAabb(Vector2 center)
        {
            //IL_000b: Unknown result type (might be due to invalid IL or missing references)
            return CreateAABB(center, 1f / 30f, 1f / 30f);
        }

        public static AABB CreateAABB(Vector2 center, float width, float height)
        {
            //IL_0002: Unknown result type (might be due to invalid IL or missing references)
            //IL_0062: Unknown result type (might be due to invalid IL or missing references)
            AABB result = new()
            {
                LowerBound = center,
                UpperBound = center
            };
            result.LowerBound -= new Vector2(width / 2f, height / 2f);
            result.UpperBound += new Vector2(width / 2f, height / 2f);
            return result;
        }

        public static void CreateLevelBorders(Body groundBody, Vector2 size, Borders borders)
        {
            if (borders.Bottom)
            {
                CreateBorder(groundBody, new Vector2((0f - size.X) / 2f, 0f), new Vector2(size.X * 1.5f, 0f), new Vector2(0f, -1f / 6f));
            }
            if (borders.Top)
            {
                CreateBorder(groundBody, new Vector2((0f - size.X) / 2f, size.Y), new Vector2(size.X * 1.5f, size.Y), new Vector2(0f, 1f / 6f));
            }
            if (borders.Left)
            {
                CreateBorder(groundBody, new Vector2(0f, size.Y * 1.5f), new Vector2(0f, (0f - size.Y) / 2f), new Vector2(-1f / 6f, 0f));
            }
            if (borders.Right)
            {
                CreateBorder(groundBody, new Vector2(size.X, size.Y * 1.5f), new Vector2(size.X, (0f - size.Y) / 2f), new Vector2(1f / 6f, 0f));
            }
        }

        public static void CreateBorder(Body groundBody, Vector2 start, Vector2 end, Vector2 direction)
        {
            for (int i = 0; i < 2; i++)
            {
                _ = FixtureFactory.AttachEdge(start, end, groundBody, null);
                start += direction;
                end += direction;
            }
        }
    }
}
