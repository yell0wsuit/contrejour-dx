using System.Collections.Generic;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PolygonManipulation
{
    public static class CuttingTools
    {
        public static void SplitShape(Fixture fixture, Vector2 entryPoint, Vector2 exitPoint, out Vertices first, out Vertices second)
        {
            Vector2 localPoint = fixture.Body.GetLocalPoint(ref entryPoint);
            Vector2 localPoint2 = fixture.Body.GetLocalPoint(ref exitPoint);
            if (fixture.Shape is not PolygonShape polygonShape)
            {
                first = [];
                second = [];
                return;
            }
            foreach (Vector2 vertex in polygonShape.Vertices)
            {
                if (vertex.Equals(localPoint))
                {
                    localPoint -= new Vector2(0f, 1.1920929E-07f);
                }
                if (vertex.Equals(localPoint2))
                {
                    localPoint2 += new Vector2(0f, 1.1920929E-07f);
                }
            }
            Vertices vertices = [.. polygonShape.Vertices];
            Vertices[] array = new Vertices[2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new Vertices(vertices.Count);
            }
            int[] array2 = [-1, -1];
            int num = -1;
            for (int j = 0; j < vertices.Count; j++)
            {
                int num2 = (!(Vector2.Dot(MathUtils.Cross(localPoint2 - localPoint, 1f), vertices[j] - localPoint) > 1.1920929E-07f)) ? 1 : 0;
                if (num != num2)
                {
                    if (num == 0)
                    {
                        array2[0] = array[num].Count;
                        array[num].Add(localPoint2);
                        array[num].Add(localPoint);
                    }
                    if (num == 1)
                    {
                        array2[num] = array[num].Count;
                        array[num].Add(localPoint);
                        array[num].Add(localPoint2);
                    }
                }
                array[num2].Add(vertices[j]);
                num = num2;
            }
            if (array2[0] == -1)
            {
                array2[0] = array[0].Count;
                array[0].Add(localPoint2);
                array[0].Add(localPoint);
            }
            if (array2[1] == -1)
            {
                array2[1] = array[1].Count;
                array[1].Add(localPoint);
                array[1].Add(localPoint2);
            }
            for (int k = 0; k < 2; k++)
            {
                Vector2 vector = (array2[k] <= 0) ? (array[k][^1] - array[k][0]) : (array[k][array2[k] - 1] - array[k][array2[k]]);
                vector.Normalize();
                if (!vector.IsValid())
                {
                    vector = Vector2.One;
                }
                array[k][array2[k]] += 1.1920929E-07f * vector;
                vector = (array2[k] >= array[k].Count - 2) ? (array[k][0] - array[k][^1]) : (array[k][array2[k] + 2] - array[k][array2[k] + 1]);
                vector.Normalize();
                if (!vector.IsValid())
                {
                    vector = Vector2.One;
                }
                array[k][array2[k] + 1] += 1.1920929E-07f * vector;
            }
            first = array[0];
            second = array[1];
        }

        public static bool Cut(World world, Vector2 start, Vector2 end)
        {
            List<Fixture> fixtures = [];
            List<Vector2> entryPoints = [];
            List<Vector2> exitPoints = [];
            if (world.TestPoint(start) != null || world.TestPoint(end) != null)
            {
                return false;
            }
            world.RayCast(delegate (Fixture f, Vector2 p, Vector2 n, float fr)
            {
                fixtures.Add(f);
                entryPoints.Add(p);
                return 1f;
            }, start, end);
            world.RayCast(delegate (Fixture f, Vector2 p, Vector2 n, float fr)
            {
                exitPoints.Add(p);
                return 1f;
            }, end, start);
            if (entryPoints.Count + exitPoints.Count < 2)
            {
                return false;
            }
            for (int num = 0; num < fixtures.Count; num++)
            {
                if (fixtures[num].Shape.ShapeType == ShapeType.Polygon && fixtures[num].Body.BodyType != BodyType.Static)
                {
                    SplitShape(fixtures[num], entryPoints[num], exitPoints[num], out Vertices first, out Vertices second);
                    if (first.CheckPolygon() == PolygonError.NoError)
                    {
                        Body body = BodyFactory.CreatePolygon(world, first, fixtures[num].Shape.Density, fixtures[num].Body.Position);
                        body.Rotation = fixtures[num].Body.Rotation;
                        body.LinearVelocity = fixtures[num].Body.LinearVelocity;
                        body.AngularVelocity = fixtures[num].Body.AngularVelocity;
                        body.BodyType = BodyType.Dynamic;
                    }
                    if (second.CheckPolygon() == PolygonError.NoError)
                    {
                        Body body2 = BodyFactory.CreatePolygon(world, second, fixtures[num].Shape.Density, fixtures[num].Body.Position);
                        body2.Rotation = fixtures[num].Body.Rotation;
                        body2.LinearVelocity = fixtures[num].Body.LinearVelocity;
                        body2.AngularVelocity = fixtures[num].Body.AngularVelocity;
                        body2.BodyType = BodyType.Dynamic;
                    }
                    world.RemoveBody(fixtures[num].Body);
                }
            }
            return true;
        }
    }
}
