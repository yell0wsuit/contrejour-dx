using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class MotorJoint : Joint
{
    private Vector2 _linearOffset;

    private float _angularOffset;

    private Vector2 _linearImpulse;

    private float _angularImpulse;

    private float _maxForce;

    private float _maxTorque;

    private int _indexA;

    private int _indexB;

    private Vector2 _rA;

    private Vector2 _rB;

    private Vector2 _localCenterA;

    private Vector2 _localCenterB;

    private Vector2 _linearError;

    private float _angularError;

    private float _invMassA;

    private float _invMassB;

    private float _invIA;

    private float _invIB;

    private Mat22 _linearMass;

    private float _angularMass;

    public override Vector2 WorldAnchorA
    {
        get
        {
            return base.BodyA.Position;
        }
        set
        {
        }
    }

    public override Vector2 WorldAnchorB
    {
        get
        {
            return base.BodyB.Position;
        }
        set
        {
        }
    }

    public float MaxForce
    {
        get
        {
            return _maxForce;
        }
        set
        {
            _maxForce = value;
        }
    }

    public float MaxTorque
    {
        get
        {
            return _maxTorque;
        }
        set
        {
            _maxTorque = value;
        }
    }

    public Vector2 LinearOffset
    {
        get
        {
            return _linearOffset;
        }
        set
        {
            if (_linearOffset.X != value.X || _linearOffset.Y != value.Y)
            {
                WakeBodies();
                _linearOffset = value;
            }
        }
    }

    public float AngularOffset
    {
        get
        {
            return _angularOffset;
        }
        set
        {
            if (_angularOffset != value)
            {
                WakeBodies();
                _angularOffset = value;
            }
        }
    }

    internal float CorrectionFactor { get; set; }

    internal MotorJoint()
    {
        base.JointType = JointType.Motor;
    }

    public MotorJoint(Body bodyA, Body bodyB, bool useWorldCoordinates = false)
        : base(bodyA, bodyB)
    {
        base.JointType = JointType.Motor;
        Vector2 position = base.BodyB.Position;
        if (useWorldCoordinates)
        {
            _linearOffset = base.BodyA.GetLocalPoint(position);
        }
        else
        {
            _linearOffset = position;
        }
        _angularOffset = 0f;
        _maxForce = 1f;
        _maxTorque = 1f;
        CorrectionFactor = 0.3f;
        _angularOffset = base.BodyB.Rotation - base.BodyA.Rotation;
    }

    public override Vector2 GetReactionForce(float invDt)
    {
        return invDt * _linearImpulse;
    }

    public override float GetReactionTorque(float invDt)
    {
        return invDt * _angularImpulse;
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
        _rA = MathUtils.Mul(q, -_localCenterA);
        _rB = MathUtils.Mul(q2, -_localCenterB);
        float invMassA = _invMassA;
        float invMassB = _invMassB;
        float invIA = _invIA;
        float invIB = _invIB;
        Mat22 mat = default(Mat22);
        mat.ex.X = invMassA + invMassB + invIA * _rA.Y * _rA.Y + invIB * _rB.Y * _rB.Y;
        mat.ex.Y = (0f - invIA) * _rA.X * _rA.Y - invIB * _rB.X * _rB.Y;
        mat.ey.X = mat.ex.Y;
        mat.ey.Y = invMassA + invMassB + invIA * _rA.X * _rA.X + invIB * _rB.X * _rB.X;
        _linearMass = mat.Inverse;
        _angularMass = invIA + invIB;
        if (_angularMass > 0f)
        {
            _angularMass = 1f / _angularMass;
        }
        _linearError = c2 + _rB - c - _rA - MathUtils.Mul(q, _linearOffset);
        _angularError = a2 - a - _angularOffset;
        _linearImpulse *= data.step.dtRatio;
        _angularImpulse *= data.step.dtRatio;
        Vector2 vector = new Vector2(_linearImpulse.X, _linearImpulse.Y);
        v -= invMassA * vector;
        w -= invIA * (MathUtils.Cross(_rA, vector) + _angularImpulse);
        v2 += invMassB * vector;
        w2 += invIB * (MathUtils.Cross(_rB, vector) + _angularImpulse);
        data.velocities[_indexA].v = v;
        data.velocities[_indexA].w = w;
        data.velocities[_indexB].v = v2;
        data.velocities[_indexB].w = w2;
    }

    internal override void SolveVelocityConstraints(ref SolverData data)
    {
        Vector2 v = data.velocities[_indexA].v;
        float w = data.velocities[_indexA].w;
        Vector2 v2 = data.velocities[_indexB].v;
        float w2 = data.velocities[_indexB].w;
        float invMassA = _invMassA;
        float invMassB = _invMassB;
        float invIA = _invIA;
        float invIB = _invIB;
        float dt = data.step.dt;
        float inv_dt = data.step.inv_dt;
        float num = w2 - w + inv_dt * CorrectionFactor * _angularError;
        float num2 = (0f - _angularMass) * num;
        float angularImpulse = _angularImpulse;
        float num3 = dt * _maxTorque;
        _angularImpulse = MathUtils.Clamp(_angularImpulse + num2, 0f - num3, num3);
        num2 = _angularImpulse - angularImpulse;
        w -= invIA * num2;
        w2 += invIB * num2;
        Vector2 v3 = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA) + inv_dt * CorrectionFactor * _linearError;
        Vector2 vector = -MathUtils.Mul(ref _linearMass, ref v3);
        Vector2 linearImpulse = _linearImpulse;
        _linearImpulse += vector;
        float num4 = dt * _maxForce;
        if (_linearImpulse.LengthSquared() > num4 * num4)
        {
            _linearImpulse.Normalize();
            _linearImpulse *= num4;
        }
        vector = _linearImpulse - linearImpulse;
        v -= invMassA * vector;
        w -= invIA * MathUtils.Cross(_rA, vector);
        v2 += invMassB * vector;
        w2 += invIB * MathUtils.Cross(_rB, vector);
        data.velocities[_indexA].v = v;
        data.velocities[_indexA].w = w;
        data.velocities[_indexB].v = v2;
        data.velocities[_indexB].w = w2;
    }

    internal override bool SolvePositionConstraints(ref SolverData data)
    {
        return true;
    }
}
