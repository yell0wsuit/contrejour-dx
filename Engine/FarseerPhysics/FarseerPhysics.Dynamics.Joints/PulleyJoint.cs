using System;

using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class PulleyJoint : Joint
{
    private float _impulse;

    private int _indexA;

    private int _indexB;

    private Vector2 _uA;

    private Vector2 _uB;

    private Vector2 _rA;

    private Vector2 _rB;

    private Vector2 _localCenterA;

    private Vector2 _localCenterB;

    private float _invMassA;

    private float _invMassB;

    private float _invIA;

    private float _invIB;

    private float _mass;

    public Vector2 LocalAnchorA { get; set; }

    public Vector2 LocalAnchorB { get; set; }

    public sealed override Vector2 WorldAnchorA { get; set; }

    public sealed override Vector2 WorldAnchorB { get; set; }

    public float LengthA { get; set; }

    public float LengthB { get; set; }

    public float CurrentLengthA
    {
        get
        {
            Vector2 worldPoint = base.BodyA.GetWorldPoint(LocalAnchorA);
            Vector2 worldAnchorA = WorldAnchorA;
            return (worldPoint - worldAnchorA).Length();
        }
    }

    public float CurrentLengthB
    {
        get
        {
            Vector2 worldPoint = base.BodyB.GetWorldPoint(LocalAnchorB);
            Vector2 worldAnchorB = WorldAnchorB;
            return (worldPoint - worldAnchorB).Length();
        }
    }

    public float Ratio { get; set; }

    internal float Constant { get; set; }

    internal PulleyJoint()
    {
        base.JointType = JointType.Pulley;
    }

    public PulleyJoint(Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, Vector2 worldAnchorA, Vector2 worldAnchorB, float ratio, bool useWorldCoordinates = false)
        : base(bodyA, bodyB)
    {
        base.JointType = JointType.Pulley;
        WorldAnchorA = worldAnchorA;
        WorldAnchorB = worldAnchorB;
        if (useWorldCoordinates)
        {
            LocalAnchorA = base.BodyA.GetLocalPoint(anchorA);
            LocalAnchorB = base.BodyB.GetLocalPoint(anchorB);
            LengthA = (anchorA - worldAnchorA).Length();
            LengthB = (anchorB - worldAnchorB).Length();
        }
        else
        {
            LocalAnchorA = anchorA;
            LocalAnchorB = anchorB;
            LengthA = (anchorA - base.BodyA.GetLocalPoint(worldAnchorA)).Length();
            LengthB = (anchorB - base.BodyB.GetLocalPoint(worldAnchorB)).Length();
        }
        Ratio = ratio;
        Constant = LengthA + ratio * LengthB;
        _impulse = 0f;
    }

    public override Vector2 GetReactionForce(float invDt)
    {
        Vector2 vector = _impulse * _uB;
        return invDt * vector;
    }

    public override float GetReactionTorque(float invDt)
    {
        return 0f;
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
        _rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
        _rB = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
        _uA = c + _rA - WorldAnchorA;
        _uB = c2 + _rB - WorldAnchorB;
        float num = _uA.Length();
        float num2 = _uB.Length();
        if (num > 0.049999997f)
        {
            _uA *= 1f / num;
        }
        else
        {
            _uA = Vector2.Zero;
        }
        if (num2 > 0.049999997f)
        {
            _uB *= 1f / num2;
        }
        else
        {
            _uB = Vector2.Zero;
        }
        float num3 = MathUtils.Cross(_rA, _uA);
        float num4 = MathUtils.Cross(_rB, _uB);
        float num5 = _invMassA + _invIA * num3 * num3;
        float num6 = _invMassB + _invIB * num4 * num4;
        _mass = num5 + Ratio * Ratio * num6;
        if (_mass > 0f)
        {
            _mass = 1f / _mass;
        }
        _impulse *= data.step.dtRatio;
        Vector2 vector = (0f - _impulse) * _uA;
        Vector2 vector2 = (0f - Ratio) * _impulse * _uB;
        v += _invMassA * vector;
        w += _invIA * MathUtils.Cross(_rA, vector);
        v2 += _invMassB * vector2;
        w2 += _invIB * MathUtils.Cross(_rB, vector2);
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
        Vector2 value = v + MathUtils.Cross(w, _rA);
        Vector2 value2 = v2 + MathUtils.Cross(w2, _rB);
        float num = 0f - Vector2.Dot(_uA, value) - Ratio * Vector2.Dot(_uB, value2);
        float num2 = (0f - _mass) * num;
        _impulse += num2;
        Vector2 vector = (0f - num2) * _uA;
        Vector2 vector2 = (0f - Ratio) * num2 * _uB;
        v += _invMassA * vector;
        w += _invIA * MathUtils.Cross(_rA, vector);
        v2 += _invMassB * vector2;
        w2 += _invIB * MathUtils.Cross(_rB, vector2);
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
        Vector2 vector3 = c + vector - WorldAnchorA;
        Vector2 vector4 = c2 + vector2 - WorldAnchorB;
        float num = vector3.Length();
        float num2 = vector4.Length();
        if (num > 0.049999997f)
        {
            vector3 *= 1f / num;
        }
        else
        {
            vector3 = Vector2.Zero;
        }
        if (num2 > 0.049999997f)
        {
            vector4 *= 1f / num2;
        }
        else
        {
            vector4 = Vector2.Zero;
        }
        float num3 = MathUtils.Cross(vector, vector3);
        float num4 = MathUtils.Cross(vector2, vector4);
        float num5 = _invMassA + _invIA * num3 * num3;
        float num6 = _invMassB + _invIB * num4 * num4;
        float num7 = num5 + Ratio * Ratio * num6;
        if (num7 > 0f)
        {
            num7 = 1f / num7;
        }
        float num8 = Constant - num - Ratio * num2;
        float num9 = Math.Abs(num8);
        float num10 = (0f - num7) * num8;
        Vector2 vector5 = (0f - num10) * vector3;
        Vector2 vector6 = (0f - Ratio) * num10 * vector4;
        c += _invMassA * vector5;
        a += _invIA * MathUtils.Cross(vector, vector5);
        c2 += _invMassB * vector6;
        a2 += _invIB * MathUtils.Cross(vector2, vector6);
        data.positions[_indexA].c = c;
        data.positions[_indexA].a = a;
        data.positions[_indexB].c = c2;
        data.positions[_indexB].a = a2;
        return num9 < 0.005f;
    }
}
