using System;

using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class WheelJoint : Joint
{
    private Vector2 _localYAxis;

    private float _impulse;

    private float _motorImpulse;

    private float _springImpulse;

    private float _maxMotorTorque;

    private float _motorSpeed;

    private bool _enableMotor;

    private int _indexA;

    private int _indexB;

    private Vector2 _localCenterA;

    private Vector2 _localCenterB;

    private float _invMassA;

    private float _invMassB;

    private float _invIA;

    private float _invIB;

    private Vector2 _ax;

    private Vector2 _ay;

    private float _sAx;

    private float _sBx;

    private float _sAy;

    private float _sBy;

    private float _mass;

    private float _motorMass;

    private float _springMass;

    private float _bias;

    private float _gamma;

    private Vector2 _axis;

    public Vector2 LocalAnchorA { get; set; }

    public Vector2 LocalAnchorB { get; set; }

    public override Vector2 WorldAnchorA
    {
        get
        {
            return base.BodyA.GetWorldPoint(LocalAnchorA);
        }
        set
        {
            LocalAnchorA = base.BodyA.GetLocalPoint(value);
        }
    }

    public override Vector2 WorldAnchorB
    {
        get
        {
            return base.BodyB.GetWorldPoint(LocalAnchorB);
        }
        set
        {
            LocalAnchorB = base.BodyB.GetLocalPoint(value);
        }
    }

    public Vector2 Axis
    {
        get
        {
            return _axis;
        }
        set
        {
            _axis = value;
            LocalXAxis = base.BodyA.GetLocalVector(_axis);
            _localYAxis = MathUtils.Cross(1f, LocalXAxis);
        }
    }

    public Vector2 LocalXAxis { get; private set; }

    public float MotorSpeed
    {
        get
        {
            return _motorSpeed;
        }
        set
        {
            WakeBodies();
            _motorSpeed = value;
        }
    }

    public float MaxMotorTorque
    {
        get
        {
            return _maxMotorTorque;
        }
        set
        {
            WakeBodies();
            _maxMotorTorque = value;
        }
    }

    public float Frequency { get; set; }

    public float DampingRatio { get; set; }

    public float JointTranslation
    {
        get
        {
            Body bodyA = base.BodyA;
            Body bodyB = base.BodyB;
            Vector2 worldPoint = bodyA.GetWorldPoint(LocalAnchorA);
            Vector2 worldPoint2 = bodyB.GetWorldPoint(LocalAnchorB);
            Vector2 value = worldPoint2 - worldPoint;
            Vector2 worldVector = bodyA.GetWorldVector(LocalXAxis);
            return Vector2.Dot(value, worldVector);
        }
    }

    public float JointSpeed
    {
        get
        {
            float angularVelocity = base.BodyA.AngularVelocity;
            float angularVelocity2 = base.BodyB.AngularVelocity;
            return angularVelocity2 - angularVelocity;
        }
    }

    public bool MotorEnabled
    {
        get
        {
            return _enableMotor;
        }
        set
        {
            WakeBodies();
            _enableMotor = value;
        }
    }

    internal WheelJoint()
    {
        base.JointType = JointType.Wheel;
    }

    public WheelJoint(Body bodyA, Body bodyB, Vector2 anchor, Vector2 axis, bool useWorldCoordinates = false)
        : base(bodyA, bodyB)
    {
        base.JointType = JointType.Wheel;
        if (useWorldCoordinates)
        {
            LocalAnchorA = bodyA.GetLocalPoint(anchor);
            LocalAnchorB = bodyB.GetLocalPoint(anchor);
        }
        else
        {
            LocalAnchorA = bodyA.GetLocalPoint(bodyB.GetWorldPoint(anchor));
            LocalAnchorB = anchor;
        }
        Axis = axis;
    }

    public float GetMotorTorque(float invDt)
    {
        return invDt * _motorImpulse;
    }

    public override Vector2 GetReactionForce(float invDt)
    {
        return invDt * (_impulse * _ay + _springImpulse * _ax);
    }

    public override float GetReactionTorque(float invDt)
    {
        return invDt * _motorImpulse;
    }

    internal override void InitVelocityConstraints(ref SolverData data)
    {
        _indexA = base.BodyA.IslandIndex;
        _indexB = base.BodyB.IslandIndex;
        _localCenterA = base.BodyA._sweep.LocalCenter;
        _localCenterB = base.BodyB._sweep.LocalCenter;
        _invMassA = base.BodyA._invMass;
        _invMassB = base.BodyB._invMass;
        _invIA = base.BodyA._invI;
        _invIB = base.BodyB._invI;
        float invMassA = _invMassA;
        float invMassB = _invMassB;
        float invIA = _invIA;
        float invIB = _invIB;
        Vector2 c = data.positions[_indexA].c;
        float a = data.positions[_indexA].a;
        Vector2 v = data.velocities[_indexA].v;
        float w = data.velocities[_indexA].w;
        Vector2 c2 = data.positions[_indexB].c;
        float a2 = data.positions[_indexB].a;
        Vector2 v2 = data.velocities[_indexB].v;
        float w2 = data.velocities[_indexB].w;
        Rot q = new Rot(a);
        Rot q2 = new Rot(a2);
        Vector2 vector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
        Vector2 vector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
        Vector2 vector3 = c2 + vector2 - c - vector;
        _ay = MathUtils.Mul(q, _localYAxis);
        _sAy = MathUtils.Cross(vector3 + vector, _ay);
        _sBy = MathUtils.Cross(vector2, _ay);
        _mass = invMassA + invMassB + invIA * _sAy * _sAy + invIB * _sBy * _sBy;
        if (_mass > 0f)
        {
            _mass = 1f / _mass;
        }
        _springMass = 0f;
        _bias = 0f;
        _gamma = 0f;
        if (Frequency > 0f)
        {
            _ax = MathUtils.Mul(q, LocalXAxis);
            _sAx = MathUtils.Cross(vector3 + vector, _ax);
            _sBx = MathUtils.Cross(vector2, _ax);
            float num = invMassA + invMassB + invIA * _sAx * _sAx + invIB * _sBx * _sBx;
            if (num > 0f)
            {
                _springMass = 1f / num;
                float num2 = Vector2.Dot(vector3, _ax);
                float num3 = (float)Math.PI * 2f * Frequency;
                float num4 = 2f * _springMass * DampingRatio * num3;
                float num5 = _springMass * num3 * num3;
                float dt = data.step.dt;
                _gamma = dt * (num4 + dt * num5);
                if (_gamma > 0f)
                {
                    _gamma = 1f / _gamma;
                }
                _bias = num2 * dt * num5 * _gamma;
                _springMass = num + _gamma;
                if (_springMass > 0f)
                {
                    _springMass = 1f / _springMass;
                }
            }
        }
        else
        {
            _springImpulse = 0f;
        }
        if (_enableMotor)
        {
            _motorMass = invIA + invIB;
            if (_motorMass > 0f)
            {
                _motorMass = 1f / _motorMass;
            }
        }
        else
        {
            _motorMass = 0f;
            _motorImpulse = 0f;
        }
        _impulse *= data.step.dtRatio;
        _springImpulse *= data.step.dtRatio;
        _motorImpulse *= data.step.dtRatio;
        Vector2 vector4 = _impulse * _ay + _springImpulse * _ax;
        float num6 = _impulse * _sAy + _springImpulse * _sAx + _motorImpulse;
        float num7 = _impulse * _sBy + _springImpulse * _sBx + _motorImpulse;
        v -= _invMassA * vector4;
        w -= _invIA * num6;
        v2 += _invMassB * vector4;
        w2 += _invIB * num7;
        data.velocities[_indexA].v = v;
        data.velocities[_indexA].w = w;
        data.velocities[_indexB].v = v2;
        data.velocities[_indexB].w = w2;
    }

    internal override void SolveVelocityConstraints(ref SolverData data)
    {
        float invMassA = _invMassA;
        float invMassB = _invMassB;
        float invIA = _invIA;
        float invIB = _invIB;
        Vector2 v = data.velocities[_indexA].v;
        float w = data.velocities[_indexA].w;
        Vector2 v2 = data.velocities[_indexB].v;
        float w2 = data.velocities[_indexB].w;
        float num = Vector2.Dot(_ax, v2 - v) + _sBx * w2 - _sAx * w;
        float num2 = (0f - _springMass) * (num + _bias + _gamma * _springImpulse);
        _springImpulse += num2;
        Vector2 vector = num2 * _ax;
        float num3 = num2 * _sAx;
        float num4 = num2 * _sBx;
        v -= invMassA * vector;
        w -= invIA * num3;
        v2 += invMassB * vector;
        w2 += invIB * num4;
        float num5 = w2 - w - _motorSpeed;
        float num6 = (0f - _motorMass) * num5;
        float motorImpulse = _motorImpulse;
        float num7 = data.step.dt * _maxMotorTorque;
        _motorImpulse = MathUtils.Clamp(_motorImpulse + num6, 0f - num7, num7);
        num6 = _motorImpulse - motorImpulse;
        w -= invIA * num6;
        w2 += invIB * num6;
        float num8 = Vector2.Dot(_ay, v2 - v) + _sBy * w2 - _sAy * w;
        float num9 = (0f - _mass) * num8;
        _impulse += num9;
        Vector2 vector2 = num9 * _ay;
        float num10 = num9 * _sAy;
        float num11 = num9 * _sBy;
        v -= invMassA * vector2;
        w -= invIA * num10;
        v2 += invMassB * vector2;
        w2 += invIB * num11;
        data.velocities[_indexA].v = v;
        data.velocities[_indexA].w = w;
        data.velocities[_indexB].v = v2;
        data.velocities[_indexB].w = w2;
    }

    internal override bool SolvePositionConstraints(ref SolverData data)
    {
        Vector2 c = data.positions[_indexA].c;
        float a = data.positions[_indexA].a;
        Vector2 c2 = data.positions[_indexB].c;
        float a2 = data.positions[_indexB].a;
        Rot q = new Rot(a);
        Rot q2 = new Rot(a2);
        Vector2 vector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
        Vector2 vector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
        Vector2 vector3 = c2 - c + vector2 - vector;
        Vector2 vector4 = MathUtils.Mul(q, _localYAxis);
        float num = MathUtils.Cross(vector3 + vector, vector4);
        float num2 = MathUtils.Cross(vector2, vector4);
        float num3 = Vector2.Dot(vector3, vector4);
        float num4 = _invMassA + _invMassB + _invIA * _sAy * _sAy + _invIB * _sBy * _sBy;
        float num5 = ((num4 == 0f) ? 0f : ((0f - num3) / num4));
        Vector2 vector5 = num5 * vector4;
        float num6 = num5 * num;
        float num7 = num5 * num2;
        c -= _invMassA * vector5;
        a -= _invIA * num6;
        c2 += _invMassB * vector5;
        a2 += _invIB * num7;
        data.positions[_indexA].c = c;
        data.positions[_indexA].a = a;
        data.positions[_indexB].c = c2;
        data.positions[_indexB].a = a2;
        return Math.Abs(num3) <= 0.005f;
    }
}
