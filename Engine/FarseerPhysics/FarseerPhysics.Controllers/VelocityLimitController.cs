using System;
using System.Collections.Generic;

using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Controllers;

public class VelocityLimitController : Controller
{
    public bool LimitAngularVelocity = true;

    public bool LimitLinearVelocity = true;

    private readonly List<Body> _bodies = [];

    private float _maxAngularSqared;
    private float _maxLinearSqared;

    public float MaxAngularVelocity
    {
        get;
        set
        {
            field = value;
            _maxAngularSqared = field * field;
        }
    }

    public float MaxLinearVelocity
    {
        get;
        set
        {
            field = value;
            _maxLinearSqared = field * field;
        }
    }

    public VelocityLimitController()
        : base(ControllerType.VelocityLimitController)
    {
        MaxLinearVelocity = 2f;
        MaxAngularVelocity = (float)Math.PI / 2f;
    }

    public VelocityLimitController(float maxLinearVelocity, float maxAngularVelocity)
        : base(ControllerType.VelocityLimitController)
    {
        if (maxLinearVelocity is 0f or float.MaxValue)
        {
            LimitLinearVelocity = false;
        }
        if (maxAngularVelocity is 0f or float.MaxValue)
        {
            LimitAngularVelocity = false;
        }
        MaxLinearVelocity = maxLinearVelocity;
        MaxAngularVelocity = maxAngularVelocity;
    }

    public override void Update(float dt)
    {
        foreach (Body body in _bodies)
        {
            if (!IsActiveOn(body))
            {
                continue;
            }
            if (LimitLinearVelocity)
            {
                float num = dt * body._linearVelocity.X;
                float num2 = dt * body._linearVelocity.Y;
                float num3 = (num * num) + (num2 * num2);
                if (num3 > dt * _maxLinearSqared)
                {
                    float num4 = (float)Math.Sqrt(num3);
                    float num5 = MaxLinearVelocity / num4;
                    body._linearVelocity.X *= num5;
                    body._linearVelocity.Y *= num5;
                }
            }
            if (LimitAngularVelocity)
            {
                float num6 = dt * body._angularVelocity;
                if (num6 * num6 > _maxAngularSqared)
                {
                    float num7 = MaxAngularVelocity / Math.Abs(num6);
                    body._angularVelocity *= num7;
                }
            }
        }
    }

    public void AddBody(Body body)
    {
        _bodies.Add(body);
    }

    public void RemoveBody(Body body)
    {
        _ = _bodies.Remove(body);
    }
}
