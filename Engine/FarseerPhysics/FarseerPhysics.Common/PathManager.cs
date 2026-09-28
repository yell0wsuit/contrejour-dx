using System;
using System.Collections.Generic;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public static class PathManager
{
    public enum LinkType
    {
        Revolute,
        Slider
    }

    public static void ConvertPathToEdges(Path path, Body body, int subdivisions)
    {
        Vertices vertices = path.GetVertices(subdivisions);
        if (path.Closed)
        {
            ChainShape shape = new(vertices, createLoop: true);
            _ = body.CreateFixture(shape);
            return;
        }
        for (int i = 1; i < vertices.Count; i++)
        {
            _ = body.CreateFixture(new EdgeShape(vertices[i], vertices[i - 1]));
        }
    }

    public static void ConvertPathToPolygon(Path path, Body body, float density, int subdivisions)
    {
        if (!path.Closed)
        {
            throw new ArgumentException("The path must be closed to convert to a polygon.");
        }
        List<Vector2> vertices = path.GetVertices(subdivisions);
        List<Vertices> list = Triangulate.ConvexPartition([.. vertices], TriangulationAlgorithm.Bayazit);
        foreach (Vertices item in list)
        {
            _ = body.CreateFixture(new PolygonShape(item, density));
        }
    }

    public static List<Body> EvenlyDistributeShapesAlongPath(World world, Path path, IEnumerable<Shape> shapes, BodyType type, int copies, object userData = null)
    {
        List<Vector3> list = path.SubdivideEvenly(copies);
        List<Body> list2 = [];
        for (int i = 0; i < list.Count; i++)
        {
            Body body = new(world)
            {
                BodyType = type,
                Position = new Vector2(list[i].X, list[i].Y),
                Rotation = list[i].Z,
                UserData = userData
            };
            foreach (Shape shape in shapes)
            {
                _ = body.CreateFixture(shape);
            }
            list2.Add(body);
        }
        return list2;
    }

    public static List<Body> EvenlyDistributeShapesAlongPath(World world, Path path, Shape shape, BodyType type, int copies, object userData)
    {
        List<Shape> list = [shape];
        return EvenlyDistributeShapesAlongPath(world, path, list, type, copies, userData);
    }

    public static List<Body> EvenlyDistributeShapesAlongPath(World world, Path path, Shape shape, BodyType type, int copies)
    {
        return EvenlyDistributeShapesAlongPath(world, path, shape, type, copies, null);
    }

    public static void MoveBodyOnPath(Path path, Body body, float time, float strength, float timeStep)
    {
        Vector2 position = path.GetPosition(time);
        Vector2 vector = body.Position - position;
        Vector2 vector2 = vector / timeStep * strength;
        body.LinearVelocity = -vector2;
    }

    public static List<RevoluteJoint> AttachBodiesWithRevoluteJoint(World world, List<Body> bodies, Vector2 localAnchorA, Vector2 localAnchorB, bool connectFirstAndLast, bool collideConnected)
    {
        List<RevoluteJoint> list = new(bodies.Count + 1);
        for (int i = 1; i < bodies.Count; i++)
        {
            RevoluteJoint revoluteJoint = new(bodies[i], bodies[i - 1], localAnchorA, localAnchorB)
            {
                CollideConnected = collideConnected
            };
            world.AddJoint(revoluteJoint);
            list.Add(revoluteJoint);
        }
        if (connectFirstAndLast)
        {
            RevoluteJoint revoluteJoint2 = new(bodies[0], bodies[^1], localAnchorA, localAnchorB)
            {
                CollideConnected = collideConnected
            };
            world.AddJoint(revoluteJoint2);
            list.Add(revoluteJoint2);
        }
        return list;
    }
}
