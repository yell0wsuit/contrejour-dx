using System;
using System.Collections.Generic;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Factories;

public static class FixtureFactory
{
    public static Fixture AttachEdge(Vector2 start, Vector2 end, Body body, object userData = null)
    {
        EdgeShape shape = new(start, end);
        return body.CreateFixture(shape, userData);
    }

    public static Fixture AttachChainShape(Vertices vertices, Body body, object userData = null)
    {
        ChainShape shape = new(vertices);
        return body.CreateFixture(shape, userData);
    }

    public static Fixture AttachLoopShape(Vertices vertices, Body body, object userData = null)
    {
        ChainShape shape = new(vertices, createLoop: true);
        return body.CreateFixture(shape, userData);
    }

    public static Fixture AttachRectangle(float width, float height, float density, Vector2 offset, Body body, object userData = null)
    {
        Vertices vertices = PolygonTools.CreateRectangle(width / 2f, height / 2f);
        vertices.Translate(ref offset);
        PolygonShape shape = new(vertices, density);
        return body.CreateFixture(shape, userData);
    }

    public static Fixture AttachCircle(float radius, float density, Body body, object userData = null)
    {
        if (radius <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be more than 0 meters");
        }
        CircleShape shape = new(radius, density);
        return body.CreateFixture(shape, userData);
    }

    public static Fixture AttachCircle(float radius, float density, Body body, Vector2 offset, object userData = null)
    {
        if (radius <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be more than 0 meters");
        }
        CircleShape circleShape = new(radius, density)
        {
            Position = offset
        };
        return body.CreateFixture(circleShape, userData);
    }

    public static Fixture AttachPolygon(Vertices vertices, float density, Body body, object userData = null)
    {
        if (vertices.Count <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(vertices), "Too few points to be a polygon");
        }
        PolygonShape shape = new(vertices, density);
        return body.CreateFixture(shape, userData);
    }

    public static Fixture AttachEllipse(float xRadius, float yRadius, int edges, float density, Body body, object userData = null)
    {
        if (xRadius <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(xRadius), "X-radius must be more than 0");
        }
        if (yRadius <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(yRadius), "Y-radius must be more than 0");
        }
        Vertices vertices = PolygonTools.CreateEllipse(xRadius, yRadius, edges);
        PolygonShape shape = new(vertices, density);
        return body.CreateFixture(shape, userData);
    }

    public static List<Fixture> AttachCompoundPolygon(List<Vertices> list, float density, Body body, object userData = null)
    {
        List<Fixture> list2 = new(list.Count);
        foreach (Vertices item in list)
        {
            if (item.Count == 2)
            {
                EdgeShape shape = new(item[0], item[1]);
                list2.Add(body.CreateFixture(shape, userData));
            }
            else
            {
                PolygonShape shape2 = new(item, density);
                list2.Add(body.CreateFixture(shape2, userData));
            }
        }
        return list2;
    }

    public static Fixture AttachLineArc(float radians, int sides, float radius, Vector2 position, float angle, bool closed, Body body)
    {
        Vertices vertices = PolygonTools.CreateArc(radians, sides, radius);
        vertices.Rotate((((float)Math.PI - radians) / 2f) + angle);
        vertices.Translate(ref position);
        return !closed ? AttachChainShape(vertices, body) : AttachLoopShape(vertices, body);
    }

    public static List<Fixture> AttachSolidArc(float density, float radians, int sides, float radius, Vector2 position, float angle, Body body)
    {
        Vertices vertices = PolygonTools.CreateArc(radians, sides, radius);
        vertices.Rotate((((float)Math.PI - radians) / 2f) + angle);
        vertices.Translate(ref position);
        vertices.Add(vertices[0]);
        List<Vertices> list = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip);
        return AttachCompoundPolygon(list, density, body);
    }
}
