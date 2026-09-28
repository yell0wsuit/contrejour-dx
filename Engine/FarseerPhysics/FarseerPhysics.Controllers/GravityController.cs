using System;
using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Controllers;

public class GravityController : Controller
{
    public float MinRadius { get; set; }

    public float MaxRadius { get; set; }

    public float Strength { get; set; }

    public GravityType GravityType { get; set; }

    public List<Body> Bodies { get; set; }

    public List<Vector2> Points { get; set; }

    public GravityController(float strength)
        : base(ControllerType.GravityController)
    {
        Strength = strength;
        MaxRadius = float.MaxValue;
        GravityType = GravityType.DistanceSquared;
        Points = new List<Vector2>();
        Bodies = new List<Body>();
    }

    public GravityController(float strength, float maxRadius, float minRadius)
        : base(ControllerType.GravityController)
    {
        MinRadius = minRadius;
        MaxRadius = maxRadius;
        Strength = strength;
        GravityType = GravityType.DistanceSquared;
        Points = new List<Vector2>();
        Bodies = new List<Body>();
    }

    public override void Update(float dt)
    {
        Vector2 force = Vector2.Zero;
        foreach (Body body in World.BodyList)
        {
            if (!IsActiveOn(body))
            {
                continue;
            }
            foreach (Body body2 in Bodies)
            {
                if (body == body2 || (body.IsStatic && body2.IsStatic) || !body2.Enabled)
                {
                    continue;
                }
                Vector2 vector = body2.Position - body.Position;
                float num = vector.LengthSquared();
                if (!(num <= 1.1920929E-07f) && !(num > MaxRadius * MaxRadius) && !(num < MinRadius * MinRadius))
                {
                    switch (GravityType)
                    {
                        case GravityType.DistanceSquared:
                            force = Strength / num * body.Mass * body2.Mass * vector;
                            break;
                        case GravityType.Linear:
                            force = Strength / (float)Math.Sqrt(num) * body.Mass * body2.Mass * vector;
                            break;
                    }
                    body.ApplyForce(ref force);
                }
            }
            foreach (Vector2 point in Points)
            {
                Vector2 vector2 = point - body.Position;
                float num2 = vector2.LengthSquared();
                if (!(num2 <= 1.1920929E-07f) && !(num2 > MaxRadius * MaxRadius) && !(num2 < MinRadius * MinRadius))
                {
                    switch (GravityType)
                    {
                        case GravityType.DistanceSquared:
                            force = Strength / num2 * body.Mass * vector2;
                            break;
                        case GravityType.Linear:
                            force = Strength / (float)Math.Sqrt(num2) * body.Mass * vector2;
                            break;
                    }
                    body.ApplyForce(ref force);
                }
            }
        }
    }

    public void AddBody(Body body)
    {
        Bodies.Add(body);
    }

    public void AddPoint(Vector2 point)
    {
        Points.Add(point);
    }
}
