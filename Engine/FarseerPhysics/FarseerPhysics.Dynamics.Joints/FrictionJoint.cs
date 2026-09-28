using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class FrictionJoint : Joint
{
    private Vector2 _linearImpulse;

    private float _angularImpulse;

    private int _indexA;

    private int _indexB;

    private Vector2 _rA;

    private Vector2 _rB;

    private Vector2 _localCenterA;

    private Vector2 _localCenterB;

    private float _invMassA;

    private float _invMassB;

    private float _invIA;

    private float _invIB;

    private float _angularMass;

    private Mat22 _linearMass;

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

    public float MaxForce { get; set; }

    public float MaxTorque { get; set; }

    internal FrictionJoint()
    {
        JointType = JointType.Friction;
    }

    public FrictionJoint(Body bodyA, Body bodyB, Vector2 anchor, bool useWorldCoordinates = false)
        : base(bodyA, bodyB)
    {
        JointType = JointType.Friction;
        if (useWorldCoordinates)
        {
            LocalAnchorA = BodyA.GetLocalPoint(anchor);
            LocalAnchorB = BodyB.GetLocalPoint(anchor);
        }
        else
        {
            LocalAnchorA = anchor;
            LocalAnchorB = anchor;
        }
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
        _indexA = BodyA.IslandIndex;
        _indexB = BodyB.IslandIndex;
        _localCenterA = BodyA._sweep.LocalCenter;
        _localCenterB = BodyB._sweep.LocalCenter;
        _invMassA = BodyA._invMass;
        _invMassB = BodyB._invMass;
        _invIA = BodyA._invI;
        _invIB = BodyB._invI;
        float a = data.positions[_indexA].a;
        Vector2 v = data.velocities[_indexA].v;
        float w = data.velocities[_indexA].w;
        float a2 = data.positions[_indexB].a;
        Vector2 v2 = data.velocities[_indexB].v;
        float w2 = data.velocities[_indexB].w;
        Rot q = new(a);
        Rot q2 = new(a2);
        _rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
        _rB = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
        float invMassA = _invMassA;
        float invMassB = _invMassB;
        float invIA = _invIA;
        float invIB = _invIB;
        Mat22 mat = default;
        mat.ex.X = invMassA + invMassB + (invIA * _rA.Y * _rA.Y) + (invIB * _rB.Y * _rB.Y);
        mat.ex.Y = ((0f - invIA) * _rA.X * _rA.Y) - (invIB * _rB.X * _rB.Y);
        mat.ey.X = mat.ex.Y;
        mat.ey.Y = invMassA + invMassB + (invIA * _rA.X * _rA.X) + (invIB * _rB.X * _rB.X);
        _linearMass = mat.Inverse;
        _angularMass = invIA + invIB;
        if (_angularMass > 0f)
        {
            _angularMass = 1f / _angularMass;
        }
        _linearImpulse *= data.step.dtRatio;
        _angularImpulse *= data.step.dtRatio;
        Vector2 vector = new(_linearImpulse.X, _linearImpulse.Y);
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
        float num = w2 - w;
        float num2 = (0f - _angularMass) * num;
        float angularImpulse = _angularImpulse;
        float num3 = dt * MaxTorque;
        _angularImpulse = MathUtils.Clamp(_angularImpulse + num2, 0f - num3, num3);
        num2 = _angularImpulse - angularImpulse;
        w -= invIA * num2;
        w2 += invIB * num2;
        Vector2 v3 = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA);
        Vector2 vector = -MathUtils.Mul(ref _linearMass, v3);
        Vector2 linearImpulse = _linearImpulse;
        _linearImpulse += vector;
        float num4 = dt * MaxForce;
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
