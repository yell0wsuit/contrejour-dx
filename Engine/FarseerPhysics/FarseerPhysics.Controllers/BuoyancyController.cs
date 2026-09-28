using System.Collections.Generic;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Controllers;

public sealed class BuoyancyController : Controller
{
    private float AngularDragCoefficient;

    private float Density;

    private float LinearDragCoefficient;

    private Vector2 Velocity;

    private AABB _container;

    private Vector2 _gravity;

    private Vector2 _normal;

    private float _offset;

    private readonly Dictionary<int, Body> _uniqueBodies = [];

    public AABB Container
    {
        get => _container;
        set
        {
            _container = value;
            _offset = _container.UpperBound.Y;
        }
    }

    public BuoyancyController(AABB container, float density, float linearDragCoefficient, float rotationalDragCoefficient, Vector2 gravity)
        : base(ControllerType.BuoyancyController)
    {
        Container = container;
        _normal = new Vector2(0f, 1f);
        Density = density;
        LinearDragCoefficient = linearDragCoefficient;
        AngularDragCoefficient = rotationalDragCoefficient;
        _gravity = gravity;
    }

    public override void Update(float dt)
    {
        _uniqueBodies.Clear();
        World.QueryAABB(delegate (Fixture fixture2)
        {
            if (fixture2.Body.IsStatic || !fixture2.Body.Awake)
            {
                return true;
            }
            if (!_uniqueBodies.ContainsKey(fixture2.Body.BodyId))
            {
                _uniqueBodies.Add(fixture2.Body.BodyId, fixture2.Body);
            }
            return true;
        }, ref _container);
        foreach (KeyValuePair<int, Body> uniqueBody in _uniqueBodies)
        {
            Body value = uniqueBody.Value;
            Vector2 zero = Vector2.Zero;
            Vector2 zero2 = Vector2.Zero;
            float num = 0f;
            float num2 = 0f;
            for (int num3 = 0; num3 < value.FixtureList.Count; num3++)
            {
                Fixture fixture = value.FixtureList[num3];
                if (fixture.Shape.ShapeType is ShapeType.Polygon or ShapeType.Circle)
                {
                    Shape shape = fixture.Shape;
                    float num4 = shape.ComputeSubmergedArea(ref _normal, _offset, ref value._xf, out Vector2 sc);
                    num += num4;
                    zero.X += num4 * sc.X;
                    zero.Y += num4 * sc.Y;
                    num2 += num4 * shape.Density;
                    zero2.X += num4 * sc.X * shape.Density;
                    zero2.Y += num4 * sc.Y * shape.Density;
                }
            }
            zero.X /= num;
            zero.Y /= num;
            zero2.X /= num2;
            zero2.Y /= num2;
            if (!(num < 1.1920929E-07f))
            {
                Vector2 force = (0f - Density) * num * _gravity;
                value.ApplyForce(force, zero2);
                Vector2 force2 = value.GetLinearVelocityFromWorldPoint(zero) - Velocity;
                force2 *= (0f - LinearDragCoefficient) * num;
                value.ApplyForce(force2, zero);
                value.ApplyTorque((0f - value.Inertia) / value.Mass * num * value.AngularVelocity * AngularDragCoefficient);
            }
        }
    }
}
