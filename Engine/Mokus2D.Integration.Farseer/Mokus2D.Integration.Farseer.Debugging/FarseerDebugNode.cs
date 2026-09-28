using System;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Mokus2D.Integration.Farseer.Debugging;

public class FarseerDebugNode : PrimitivesNode
{
    private const int CIRCLE_SEGMENTS = 20;

    private const int Opacity = 200;

    private readonly Color STATIC_COLOR = ColorUtil.CreateColor(255, 0, 255, 200);

    private readonly Color DYNAMIC_COLOR = ColorUtil.CreateColor(0, 255, 0, 200);

    private readonly Color KINEMATIC_COLOR = ColorUtil.CreateColor(0, 0, 255, 200);

    private readonly World world;

    private readonly float _physicsToPixels;

    public FarseerDebugNode(World world, float physicsToPixels)
    {
        this.world = world;
        _physicsToPixels = physicsToPixels;
    }

    protected override void DrawPrimitives()
    {
        foreach (Body body in world.BodyList)
        {
            foreach (Fixture fixture in body.FixtureList)
            {
                DrawFixture(fixture, body);
            }
        }
        foreach (Joint joint in world.JointList)
        {
            DrawJoint(joint);
        }
    }

    private void DrawJoint(Joint joint)
    {
        if (joint.JointType == JointType.Revolute)
        {
            DrawRevoluteJoint((RevoluteJoint)joint);
        }
    }

    private void DrawRevoluteJoint(RevoluteJoint joint)
    {
        DrawCircle(joint.WorldAnchorA, 3f / _physicsToPixels, Color.Red);
    }

    private void DrawFixture(Fixture fixture, Body body)
    {
        if (fixture.Shape.ShapeType == ShapeType.Circle)
        {
            DrawCircle((CircleShape)fixture.Shape, body, fixture);
        }
        else if (fixture.Shape.ShapeType == ShapeType.Polygon)
        {
            DrawPolygon((PolygonShape)fixture.Shape, body, fixture);
        }
        else if (fixture.Shape.ShapeType == ShapeType.Edge)
        {
            DrawEdge((EdgeShape)fixture.Shape, body, fixture);
        }
    }

    private void DrawEdge(EdgeShape shape, Body body, Fixture fixture)
    {
        VertexPositionColor[] array = new VertexPositionColor[2];
        array[0].Color = GetColor(body, fixture);
        array[1].Color = GetColor(body, fixture);
        array[0].Position = body.GetWorldPoint(shape.Vertex1).ToVector3() * _physicsToPixels;
        array[1].Position = body.GetWorldPoint(shape.Vertex2).ToVector3() * _physicsToPixels;
        GraphUtil.DrawLineList(array);
    }

    private void DrawPolygon(PolygonShape shape, Body body, Fixture fixture)
    {
        VertexPositionColor[] array = new VertexPositionColor[shape.Vertices.Count];
        Color color = GetColor(body, fixture);
        for (int i = 0; i < shape.Vertices.Count; i++)
        {
            array[i].Color = color;
            Vector2 localPoint = shape.Vertices[i % shape.Vertices.Count];
            array[i].Position = body.GetWorldPoint(localPoint).ToVector3() * _physicsToPixels;
        }
        GraphUtil.DrawTriangleFan(array);
    }

    private void DrawCircle(CircleShape shape, Body body, Fixture fixture)
    {
        Color color = GetColor(body, fixture);
        DrawCircle(body.GetWorldPoint(shape.Position), shape.Radius, color);
    }

    private void DrawCircle(Vector2 worldPosition, float radius, Color color)
    {
        VertexPositionColor[] array = new VertexPositionColor[20];
        for (int i = 0; i < 20; i++)
        {
            array[i].Color = color;
            Vector2 vector = VectorUtil.ToVector(radius, i * ((float)Math.PI * 2f) / 20f) + worldPosition;
            array[i].Position = vector.ToVector3() * _physicsToPixels;
        }
        GraphUtil.DrawTriangleFan(array);
    }

    private Color GetColor(Body body, Fixture fixture)
    {
        float num = fixture.IsSensor ? 0.5f : 1f;
        return body.BodyType switch
        {
            BodyType.Dynamic => DYNAMIC_COLOR * num,
            BodyType.Kinematic => KINEMATIC_COLOR * num,
            BodyType.Static => STATIC_COLOR * num,
            _ => throw new InvalidOperationException("Unknown body type"),
        };
    }
}
