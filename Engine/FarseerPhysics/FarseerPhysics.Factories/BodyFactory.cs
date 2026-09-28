using System;
using System.Collections.Generic;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Factories;

public static class BodyFactory
{
    public static Body CreateBody(World world, object userData = null)
    {
        return new Body(world, null, 0f, userData);
    }

    public static Body CreateBody(World world, Vector2 position, float rotation = 0f, object userData = null)
    {
        return new Body(world, position, rotation, userData);
    }

    public static Body CreateEdge(World world, Vector2 start, Vector2 end, object userData = null)
    {
        Body body = CreateBody(world);
        FixtureFactory.AttachEdge(start, end, body, userData);
        return body;
    }

    public static Body CreateChainShape(World world, Vertices vertices, object userData = null)
    {
        return CreateChainShape(world, vertices, Vector2.Zero, userData);
    }

    public static Body CreateChainShape(World world, Vertices vertices, Vector2 position, object userData = null)
    {
        Body body = CreateBody(world, position);
        FixtureFactory.AttachChainShape(vertices, body, userData);
        return body;
    }

    public static Body CreateLoopShape(World world, Vertices vertices, object userData = null)
    {
        return CreateLoopShape(world, vertices, Vector2.Zero, userData);
    }

    public static Body CreateLoopShape(World world, Vertices vertices, Vector2 position, object userData = null)
    {
        Body body = CreateBody(world, position);
        FixtureFactory.AttachLoopShape(vertices, body, userData);
        return body;
    }

    public static Body CreateRectangle(World world, float width, float height, float density, object userData = null)
    {
        return CreateRectangle(world, width, height, density, Vector2.Zero, userData);
    }

    public static Body CreateRectangle(World world, float width, float height, float density, Vector2 position, object userData = null)
    {
        if (width <= 0f)
        {
            throw new ArgumentOutOfRangeException("width", "Width must be more than 0 meters");
        }
        if (height <= 0f)
        {
            throw new ArgumentOutOfRangeException("height", "Height must be more than 0 meters");
        }
        Body body = CreateBody(world, position);
        body.UserData = userData;
        Vertices vertices = PolygonTools.CreateRectangle(width / 2f, height / 2f);
        PolygonShape shape = new PolygonShape(vertices, density);
        body.CreateFixture(shape);
        return body;
    }

    public static Body CreateCircle(World world, float radius, float density, object userData = null)
    {
        return CreateCircle(world, radius, density, Vector2.Zero, userData);
    }

    public static Body CreateCircle(World world, float radius, float density, Vector2 position, object userData = null)
    {
        Body body = CreateBody(world, position);
        FixtureFactory.AttachCircle(radius, density, body, userData);
        return body;
    }

    public static Body CreateEllipse(World world, float xRadius, float yRadius, int edges, float density, object userData = null)
    {
        return CreateEllipse(world, xRadius, yRadius, edges, density, Vector2.Zero, userData);
    }

    public static Body CreateEllipse(World world, float xRadius, float yRadius, int edges, float density, Vector2 position, object userData = null)
    {
        Body body = CreateBody(world, position);
        FixtureFactory.AttachEllipse(xRadius, yRadius, edges, density, body, userData);
        return body;
    }

    public static Body CreatePolygon(World world, Vertices vertices, float density, object userData = null)
    {
        return CreatePolygon(world, vertices, density, Vector2.Zero, userData);
    }

    public static Body CreatePolygon(World world, Vertices vertices, float density, Vector2 position, object userData = null)
    {
        Body body = CreateBody(world, position);
        FixtureFactory.AttachPolygon(vertices, density, body, userData);
        return body;
    }

    public static Body CreateCompoundPolygon(World world, List<Vertices> list, float density, object userData = null)
    {
        return CreateCompoundPolygon(world, list, density, Vector2.Zero, userData);
    }

    public static Body CreateCompoundPolygon(World world, List<Vertices> list, float density, Vector2 position, object userData = null)
    {
        Body body = CreateBody(world, position);
        FixtureFactory.AttachCompoundPolygon(list, density, body, userData);
        return body;
    }

    public static Body CreateGear(World world, float radius, int numberOfTeeth, float tipPercentage, float toothHeight, float density, object userData = null)
    {
        Vertices vertices = PolygonTools.CreateGear(radius, numberOfTeeth, tipPercentage, toothHeight);
        if (!vertices.IsConvex())
        {
            List<Vertices> list = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip);
            return CreateCompoundPolygon(world, list, density, userData);
        }
        return CreatePolygon(world, vertices, density, userData);
    }

    public static Body CreateCapsule(World world, float height, float topRadius, int topEdges, float bottomRadius, int bottomEdges, float density, Vector2 position, object userData = null)
    {
        Vertices vertices = PolygonTools.CreateCapsule(height, topRadius, topEdges, bottomRadius, bottomEdges);
        Body body;
        if (vertices.Count >= Settings.MaxPolygonVertices)
        {
            List<Vertices> list = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip);
            body = CreateCompoundPolygon(world, list, density, userData);
            body.Position = position;
            return body;
        }
        body = CreatePolygon(world, vertices, density, userData);
        body.Position = position;
        return body;
    }

    public static Body CreateCapsule(World world, float height, float endRadius, float density, object userData = null)
    {
        Vertices item = PolygonTools.CreateRectangle(endRadius, height / 2f);
        List<Vertices> list = new List<Vertices>();
        list.Add(item);
        Body body = CreateCompoundPolygon(world, list, density, userData);
        body.UserData = userData;
        CircleShape circleShape = new CircleShape(endRadius, density);
        circleShape.Position = new Vector2(0f, height / 2f);
        body.CreateFixture(circleShape);
        CircleShape circleShape2 = new CircleShape(endRadius, density);
        circleShape2.Position = new Vector2(0f, 0f - height / 2f);
        body.CreateFixture(circleShape2);
        return body;
    }

    public static Body CreateRoundedRectangle(World world, float width, float height, float xRadius, float yRadius, int segments, float density, Vector2 position, object userData = null)
    {
        Vertices vertices = PolygonTools.CreateRoundedRectangle(width, height, xRadius, yRadius, segments);
        if (vertices.Count >= Settings.MaxPolygonVertices)
        {
            List<Vertices> list = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip);
            Body body = CreateCompoundPolygon(world, list, density, userData);
            body.Position = position;
            return body;
        }
        return CreatePolygon(world, vertices, density);
    }

    public static Body CreateRoundedRectangle(World world, float width, float height, float xRadius, float yRadius, int segments, float density, object userData = null)
    {
        return CreateRoundedRectangle(world, width, height, xRadius, yRadius, segments, density, Vector2.Zero, userData);
    }

    public static BreakableBody CreateBreakableBody(World world, Vertices vertices, float density)
    {
        return CreateBreakableBody(world, vertices, density, Vector2.Zero);
    }

    public static BreakableBody CreateBreakableBody(World world, IEnumerable<Shape> shapes)
    {
        return CreateBreakableBody(world, shapes, Vector2.Zero);
    }

    public static BreakableBody CreateBreakableBody(World world, Vertices vertices, float density, Vector2 position)
    {
        List<Vertices> vertices2 = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip);
        BreakableBody breakableBody = new BreakableBody(vertices2, world, density);
        breakableBody.MainBody.Position = position;
        world.AddBreakableBody(breakableBody);
        return breakableBody;
    }

    public static BreakableBody CreateBreakableBody(World world, IEnumerable<Shape> shapes, Vector2 position)
    {
        BreakableBody breakableBody = new BreakableBody(shapes, world);
        breakableBody.MainBody.Position = position;
        world.AddBreakableBody(breakableBody);
        return breakableBody;
    }

    public static Body CreateLineArc(World world, float radians, int sides, float radius, Vector2 position, float angle, bool closed)
    {
        Body body = CreateBody(world);
        FixtureFactory.AttachLineArc(radians, sides, radius, position, angle, closed, body);
        return body;
    }

    public static Body CreateSolidArc(World world, float density, float radians, int sides, float radius, Vector2 position, float angle)
    {
        Body body = CreateBody(world);
        FixtureFactory.AttachSolidArc(density, radians, sides, radius, position, angle, body);
        return body;
    }
}
