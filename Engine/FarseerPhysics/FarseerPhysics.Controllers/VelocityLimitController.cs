using System;
using System.Collections.Generic;

using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Controllers;

public class VelocityLimitController : Controller
{
    public bool LimitAngularVelocity = true;

    public bool LimitLinearVelocity = true;

    private List<Body> _bodies = new List<Body>();

    private float _maxAngularSqared;

    private float _maxAngularVelocity;

    private float _maxLinearSqared;

    private float _maxLinearVelocity;

    public float MaxAngularVelocity
    {
        get
        {
            return _maxAngularVelocity;
        }
        set
        {
            _maxAngularVelocity = value;
            _maxAngularSqared = _maxAngularVelocity * _maxAngularVelocity;
        }
    }

    public float MaxLinearVelocity
    {
        get
        {
            return _maxLinearVelocity;
        }
        set
        {
            _maxLinearVelocity = value;
            _maxLinearSqared = _maxLinearVelocity * _maxLinearVelocity;
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
        if (maxLinearVelocity == 0f || maxLinearVelocity == float.MaxValue)
        {
            LimitLinearVelocity = false;
        }
        if (maxAngularVelocity == 0f || maxAngularVelocity == float.MaxValue)
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
                float num3 = num * num + num2 * num2;
                if (num3 > dt * _maxLinearSqared)
                {
                    float num4 = (float)Math.Sqrt(num3);
                    float num5 = _maxLinearVelocity / num4;
                    body._linearVelocity.X *= num5;
                    body._linearVelocity.Y *= num5;
                }
            }
            if (LimitAngularVelocity)
            {
                float num6 = dt * body._angularVelocity;
                if (num6 * num6 > _maxAngularSqared)
                {
                    float num7 = _maxAngularVelocity / Math.Abs(num6);
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
        _bodies.Remove(body);
    }
}
