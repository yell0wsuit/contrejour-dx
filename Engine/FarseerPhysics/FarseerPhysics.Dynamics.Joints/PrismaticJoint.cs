using System;

using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class PrismaticJoint : Joint
{
    private Vector2 _localYAxisA;

    private Vector3 _impulse;

    private float _lowerTranslation;

    private float _upperTranslation;

    private float _maxMotorForce;

    private float _motorSpeed;

    private bool _enableLimit;

    private bool _enableMotor;

    private LimitState _limitState;

    private int _indexA;

    private int _indexB;

    private Vector2 _localCenterA;

    private Vector2 _localCenterB;

    private float _invMassA;

    private float _invMassB;

    private float _invIA;

    private float _invIB;

    private Vector2 _axis;

    private Vector2 _perp;

    private float _s1;

    private float _s2;

    private float _a1;

    private float _a2;

    private Mat33 _K;

    private float _motorMass;

    private Vector2 _axis1;

    public Vector2 LocalAnchorA { get; set; }

    public Vector2 LocalAnchorB { get; set; }

    public override Vector2 WorldAnchorA
    {
        get => BodyA.GetWorldPoint(LocalAnchorA);
        set => LocalAnchorA = BodyA.GetLocalPoint(value);
    }

    public override Vector2 WorldAnchorB
    {
        get => BodyB.GetWorldPoint(LocalAnchorB);
        set => LocalAnchorB = BodyB.GetLocalPoint(value);
    }

    public float JointTranslation
    {
        get
        {
            Vector2 value = BodyB.GetWorldPoint(LocalAnchorB) - BodyA.GetWorldPoint(LocalAnchorA);
            Vector2 worldVector = BodyA.GetWorldVector(LocalXAxis);
            return Vector2.Dot(value, worldVector);
        }
    }

    public float JointSpeed
    {
        get
        {
            BodyA.GetTransform(out Transform transform);
            BodyB.GetTransform(out Transform transform2);
            Vector2 vector = MathUtils.Mul(ref transform.q, LocalAnchorA - BodyA.LocalCenter);
            Vector2 vector2 = MathUtils.Mul(ref transform2.q, LocalAnchorB - BodyB.LocalCenter);
            Vector2 vector3 = BodyA._sweep.C + vector;
            Vector2 vector4 = BodyB._sweep.C + vector2;
            Vector2 value = vector4 - vector3;
            Vector2 worldVector = BodyA.GetWorldVector(LocalXAxis);
            Vector2 linearVelocity = BodyA._linearVelocity;
            Vector2 linearVelocity2 = BodyB._linearVelocity;
            float angularVelocity = BodyA._angularVelocity;
            float angularVelocity2 = BodyB._angularVelocity;
            return Vector2.Dot(value, MathUtils.Cross(angularVelocity, worldVector)) + Vector2.Dot(worldVector, linearVelocity2 + MathUtils.Cross(angularVelocity2, vector2) - linearVelocity - MathUtils.Cross(angularVelocity, vector));
        }
    }

    public bool LimitEnabled
    {
        get => _enableLimit;
        set
        {
            if (value != _enableLimit)
            {
                WakeBodies();
                _enableLimit = value;
                _impulse.Z = 0f;
            }
        }
    }

    public float LowerLimit
    {
        get => _lowerTranslation;
        set
        {
            if (value != _lowerTranslation)
            {
                WakeBodies();
                _lowerTranslation = value;
                _impulse.Z = 0f;
            }
        }
    }

    public float UpperLimit
    {
        get => _upperTranslation;
        set
        {
            if (value != _upperTranslation)
            {
                WakeBodies();
                _upperTranslation = value;
                _impulse.Z = 0f;
            }
        }
    }

    public bool MotorEnabled
    {
        get => _enableMotor;
        set
        {
            WakeBodies();
            _enableMotor = value;
        }
    }

    public float MotorSpeed
    {
        get => _motorSpeed;
        set
        {
            WakeBodies();
            _motorSpeed = value;
        }
    }

    public float MaxMotorForce
    {
        get => _maxMotorForce;
        set
        {
            WakeBodies();
            _maxMotorForce = value;
        }
    }

    public float MotorImpulse { get; set; }

    public Vector2 Axis
    {
        get => _axis1;
        set
        {
            _axis1 = value;
            LocalXAxis = BodyA.GetLocalVector(_axis1);
            LocalXAxis.Normalize();
            _localYAxisA = MathUtils.Cross(1f, LocalXAxis);
        }
    }

    public Vector2 LocalXAxis { get; private set; }

    public float ReferenceAngle { get; set; }

    internal PrismaticJoint()
    {
        JointType = JointType.Prismatic;
    }

    public PrismaticJoint(Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, Vector2 axis, bool useWorldCoordinates = false)
        : base(bodyA, bodyB)
    {
        Initialize(anchorA, anchorB, axis, useWorldCoordinates);
    }

    public PrismaticJoint(Body bodyA, Body bodyB, Vector2 anchor, Vector2 axis, bool useWorldCoordinates = false)
        : base(bodyA, bodyB)
    {
        Initialize(anchor, anchor, axis, useWorldCoordinates);
    }

    private void Initialize(Vector2 localAnchorA, Vector2 localAnchorB, Vector2 axis, bool useWorldCoordinates)
    {
        JointType = JointType.Prismatic;
        if (useWorldCoordinates)
        {
            LocalAnchorA = BodyA.GetLocalPoint(localAnchorA);
            LocalAnchorB = BodyB.GetLocalPoint(localAnchorB);
        }
        else
        {
            LocalAnchorA = localAnchorA;
            LocalAnchorB = localAnchorB;
        }
        Axis = axis;
        ReferenceAngle = BodyB.Rotation - BodyA.Rotation;
        _limitState = LimitState.Inactive;
    }

    public void SetLimits(float lower, float upper)
    {
        if (upper != _upperTranslation || lower != _lowerTranslation)
        {
            WakeBodies();
            _upperTranslation = upper;
            _lowerTranslation = lower;
            _impulse.Z = 0f;
        }
    }

    public float GetMotorForce(float invDt)
    {
        return invDt * MotorImpulse;
    }

    public override Vector2 GetReactionForce(float invDt)
    {
        return invDt * ((_impulse.X * _perp) + ((MotorImpulse + _impulse.Z) * _axis));
    }

    public override float GetReactionTorque(float invDt)
    {
        return invDt * _impulse.Y;
    }

    internal override void InitVelocityConstraints(ref SolverData data)
    {
        _indexA = BodyA.IslandIndex;
        _indexB = BodyB.IslandIndex;
        _localCenterA = BodyA._sweep.LocalCenter;
        _localCenterB = BodyB._sweep.LocalCenter;
        _invMassA = BodyA._invMass;
        _invMassB = BodyB._invMass;
        _invIA = BodyA._invI;
        _invIB = BodyB._invI;
        Vector2 c = data.positions[_indexA].c;
        float a = data.positions[_indexA].a;
        Vector2 v = data.velocities[_indexA].v;
        float w = data.velocities[_indexA].w;
        Vector2 c2 = data.positions[_indexB].c;
        float a2 = data.positions[_indexB].a;
        Vector2 v2 = data.velocities[_indexB].v;
        float w2 = data.velocities[_indexB].w;
        Rot q = new(a);
        Rot q2 = new(a2);
        Vector2 vector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
        Vector2 vector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
        Vector2 vector3 = c2 - c + vector2 - vector;
        float invMassA = _invMassA;
        float invMassB = _invMassB;
        float invIA = _invIA;
        float invIB = _invIB;
        _axis = MathUtils.Mul(q, LocalXAxis);
        _a1 = MathUtils.Cross(vector3 + vector, _axis);
        _a2 = MathUtils.Cross(vector2, _axis);
        _motorMass = invMassA + invMassB + (invIA * _a1 * _a1) + (invIB * _a2 * _a2);
        if (_motorMass > 0f)
        {
            _motorMass = 1f / _motorMass;
        }
        _perp = MathUtils.Mul(q, _localYAxisA);
        _s1 = MathUtils.Cross(vector3 + vector, _perp);
        _s2 = MathUtils.Cross(vector2, _perp);
        float x = invMassA + invMassB + (invIA * _s1 * _s1) + (invIB * _s2 * _s2);
        float num = (invIA * _s1) + (invIB * _s2);
        float num2 = (invIA * _s1 * _a1) + (invIB * _s2 * _a2);
        float num3 = invIA + invIB;
        if (num3 == 0f)
        {
            num3 = 1f;
        }
        float num4 = (invIA * _a1) + (invIB * _a2);
        float z = invMassA + invMassB + (invIA * _a1 * _a1) + (invIB * _a2 * _a2);
        _K.ex = new Vector3(x, num, num2);
        _K.ey = new Vector3(num, num3, num4);
        _K.ez = new Vector3(num2, num4, z);
        if (_enableLimit)
        {
            float num5 = Vector2.Dot(_axis, vector3);
            if (Math.Abs(_upperTranslation - _lowerTranslation) < 0.01f)
            {
                _limitState = LimitState.Equal;
            }
            else if (num5 <= _lowerTranslation)
            {
                if (_limitState != LimitState.AtLower)
                {
                    _limitState = LimitState.AtLower;
                    _impulse.Z = 0f;
                }
            }
            else if (num5 >= _upperTranslation)
            {
                if (_limitState != LimitState.AtUpper)
                {
                    _limitState = LimitState.AtUpper;
                    _impulse.Z = 0f;
                }
            }
            else
            {
                _limitState = LimitState.Inactive;
                _impulse.Z = 0f;
            }
        }
        else
        {
            _limitState = LimitState.Inactive;
            _impulse.Z = 0f;
        }
        if (!_enableMotor)
        {
            MotorImpulse = 0f;
        }
        _impulse *= data.step.dtRatio;
        MotorImpulse *= data.step.dtRatio;
        Vector2 vector4 = (_impulse.X * _perp) + ((MotorImpulse + _impulse.Z) * _axis);
        float num6 = (_impulse.X * _s1) + _impulse.Y + ((MotorImpulse + _impulse.Z) * _a1);
        float num7 = (_impulse.X * _s2) + _impulse.Y + ((MotorImpulse + _impulse.Z) * _a2);
        v -= invMassA * vector4;
        w -= invIA * num6;
        v2 += invMassB * vector4;
        w2 += invIB * num7;
        data.velocities[_indexA].v = v;
        data.velocities[_indexA].w = w;
        data.velocities[_indexB].v = v2;
        data.velocities[_indexB].w = w2;
    }

    internal override void SolveVelocityConstraints(ref SolverData data)
    {
        Vector2 v = data.velocities[_indexA].v;
        float num = data.velocities[_indexA].w;
        Vector2 v2 = data.velocities[_indexB].v;
        float num2 = data.velocities[_indexB].w;
        float invMassA = _invMassA;
        float invMassB = _invMassB;
        float invIA = _invIA;
        float invIB = _invIB;
        if (_enableMotor && _limitState != LimitState.Equal)
        {
            float num3 = Vector2.Dot(_axis, v2 - v) + (_a2 * num2) - (_a1 * num);
            float num4 = _motorMass * (_motorSpeed - num3);
            float motorImpulse = MotorImpulse;
            float num5 = data.step.dt * _maxMotorForce;
            MotorImpulse = MathUtils.Clamp(MotorImpulse + num4, 0f - num5, num5);
            num4 = MotorImpulse - motorImpulse;
            Vector2 vector = num4 * _axis;
            float num6 = num4 * _a1;
            float num7 = num4 * _a2;
            v -= invMassA * vector;
            num -= invIA * num6;
            v2 += invMassB * vector;
            num2 += invIB * num7;
        }
        Vector2 vector2 = new()
        {
            X = Vector2.Dot(_perp, v2 - v) + (_s2 * num2) - (_s1 * num),
            Y = num2 - num
        };
        if (_enableLimit && _limitState != LimitState.Inactive)
        {
            float z = Vector2.Dot(_axis, v2 - v) + (_a2 * num2) - (_a1 * num);
            Vector3 vector3 = new(vector2.X, vector2.Y, z);
            Vector3 impulse = _impulse;
            Vector3 vector4 = _K.Solve33(-vector3);
            _impulse += vector4;
            if (_limitState == LimitState.AtLower)
            {
                _impulse.Z = Math.Max(_impulse.Z, 0f);
            }
            else if (_limitState == LimitState.AtUpper)
            {
                _impulse.Z = Math.Min(_impulse.Z, 0f);
            }
            Vector2 b = -vector2 - ((_impulse.Z - impulse.Z) * new Vector2(_K.ez.X, _K.ez.Y));
            Vector2 vector5 = _K.Solve22(b) + new Vector2(impulse.X, impulse.Y);
            _impulse.X = vector5.X;
            _impulse.Y = vector5.Y;
            vector4 = _impulse - impulse;
            Vector2 vector6 = (vector4.X * _perp) + (vector4.Z * _axis);
            float num8 = (vector4.X * _s1) + vector4.Y + (vector4.Z * _a1);
            float num9 = (vector4.X * _s2) + vector4.Y + (vector4.Z * _a2);
            v -= invMassA * vector6;
            num -= invIA * num8;
            v2 += invMassB * vector6;
            num2 += invIB * num9;
        }
        else
        {
            Vector2 vector7 = _K.Solve22(-vector2);
            _impulse.X += vector7.X;
            _impulse.Y += vector7.Y;
            Vector2 vector8 = vector7.X * _perp;
            float num10 = (vector7.X * _s1) + vector7.Y;
            float num11 = (vector7.X * _s2) + vector7.Y;
            v -= invMassA * vector8;
            num -= invIA * num10;
            v2 += invMassB * vector8;
            num2 += invIB * num11;
        }
        data.velocities[_indexA].v = v;
        data.velocities[_indexA].w = num;
        data.velocities[_indexB].v = v2;
        data.velocities[_indexB].w = num2;
    }

    internal override bool SolvePositionConstraints(ref SolverData data)
    {
        Vector2 c = data.positions[_indexA].c;
        float a = data.positions[_indexA].a;
        Vector2 c2 = data.positions[_indexB].c;
        float a2 = data.positions[_indexB].a;
        Rot q = new(a);
        Rot q2 = new(a2);
        float invMassA = _invMassA;
        float invMassB = _invMassB;
        float invIA = _invIA;
        float invIB = _invIB;
        Vector2 vector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
        Vector2 vector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
        Vector2 vector3 = c2 + vector2 - c - vector;
        Vector2 vector4 = MathUtils.Mul(q, LocalXAxis);
        float num = MathUtils.Cross(vector3 + vector, vector4);
        float num2 = MathUtils.Cross(vector2, vector4);
        Vector2 vector5 = MathUtils.Mul(q, _localYAxisA);
        float num3 = MathUtils.Cross(vector3 + vector, vector5);
        float num4 = MathUtils.Cross(vector2, vector5);
        Vector2 vector6 = new()
        {
            X = Vector2.Dot(vector5, vector3),
            Y = a2 - a - ReferenceAngle
        };
        float num5 = Math.Abs(vector6.X);
        float num6 = Math.Abs(vector6.Y);
        bool flag = false;
        float z = 0f;
        if (_enableLimit)
        {
            float num7 = Vector2.Dot(vector4, vector3);
            if (Math.Abs(_upperTranslation - _lowerTranslation) < 0.01f)
            {
                z = MathUtils.Clamp(num7, -0.2f, 0.2f);
                num5 = Math.Max(num5, Math.Abs(num7));
                flag = true;
            }
            else if (num7 <= _lowerTranslation)
            {
                z = MathUtils.Clamp(num7 - _lowerTranslation + 0.005f, -0.2f, 0f);
                num5 = Math.Max(num5, _lowerTranslation - num7);
                flag = true;
            }
            else if (num7 >= _upperTranslation)
            {
                z = MathUtils.Clamp(num7 - _upperTranslation - 0.005f, 0f, 0.2f);
                num5 = Math.Max(num5, num7 - _upperTranslation);
                flag = true;
            }
        }
        Vector3 vector7;
        if (flag)
        {
            float x = invMassA + invMassB + (invIA * num3 * num3) + (invIB * num4 * num4);
            float num8 = (invIA * num3) + (invIB * num4);
            float num9 = (invIA * num3 * num) + (invIB * num4 * num2);
            float num10 = invIA + invIB;
            if (num10 == 0f)
            {
                num10 = 1f;
            }
            float num11 = (invIA * num) + (invIB * num2);
            float z2 = invMassA + invMassB + (invIA * num * num) + (invIB * num2 * num2);
            vector7 = new Mat33
            {
                ex = new Vector3(x, num8, num9),
                ey = new Vector3(num8, num10, num11),
                ez = new Vector3(num9, num11, z2)
            }.Solve33(-new Vector3
            {
                X = vector6.X,
                Y = vector6.Y,
                Z = z
            });
        }
        else
        {
            float x2 = invMassA + invMassB + (invIA * num3 * num3) + (invIB * num4 * num4);
            float num12 = (invIA * num3) + (invIB * num4);
            float num13 = invIA + invIB;
            if (num13 == 0f)
            {
                num13 = 1f;
            }
            Vector2 vector8 = new Mat22
            {
                ex = new Vector2(x2, num12),
                ey = new Vector2(num12, num13)
            }.Solve(-vector6);
            vector7 = new Vector3
            {
                X = vector8.X,
                Y = vector8.Y,
                Z = 0f
            };
        }
        Vector2 vector9 = (vector7.X * vector5) + (vector7.Z * vector4);
        float num14 = (vector7.X * num3) + vector7.Y + (vector7.Z * num);
        float num15 = (vector7.X * num4) + vector7.Y + (vector7.Z * num2);
        c -= invMassA * vector9;
        a -= invIA * num14;
        c2 += invMassB * vector9;
        a2 += invIB * num15;
        data.positions[_indexA].c = c;
        data.positions[_indexA].a = a;
        data.positions[_indexB].c = c2;
        data.positions[_indexB].a = a2;
        return num5 <= 0.005f ? num6 <= (float)Math.PI / 90f : false;
    }
}
