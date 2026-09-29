using System;
using System.Numerics;

using FarseerPhysics.Common;

namespace FarseerPhysics.Dynamics.Joints
{
    public class DistanceJoint : Joint
    {
        private float _bias;

        private float _gamma;

        private float _impulse;

        private int _indexA;

        private int _indexB;

        private Vector2 _u;

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

        public sealed override Vector2 WorldAnchorA
        {
            get => BodyA.GetWorldPoint(LocalAnchorA);
            set
            {
            }
        }

        public sealed override Vector2 WorldAnchorB
        {
            get => BodyB.GetWorldPoint(LocalAnchorB);
            set
            {
            }
        }

        public float Length { get; set; }

        public float Frequency { get; set; }

        public float DampingRatio { get; set; }

        internal DistanceJoint()
        {
            JointType = JointType.Distance;
        }

        public DistanceJoint(Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, bool useWorldCoordinates = false)
            : base(bodyA, bodyB)
        {
            JointType = JointType.Distance;
            if (useWorldCoordinates)
            {
                LocalAnchorA = bodyA.GetLocalPoint(ref anchorA);
                LocalAnchorB = bodyB.GetLocalPoint(ref anchorB);
                Length = (anchorB - anchorA).Length();
            }
            else
            {
                LocalAnchorA = anchorA;
                LocalAnchorB = anchorB;
                Length = (BodyB.GetWorldPoint(ref anchorB) - BodyA.GetWorldPoint(ref anchorA)).Length();
            }
        }

        public override Vector2 GetReactionForce(float invDt)
        {
            return invDt * _impulse * _u;
        }

        public override float GetReactionTorque(float invDt)
        {
            return 0f;
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
            _rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
            _rB = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
            _u = c2 + _rB - c - _rA;
            float num = _u.Length();
            if (num > 0.005f)
            {
                _u *= 1f / num;
            }
            else
            {
                _u = Vector2.Zero;
            }
            float num2 = MathUtils.Cross(_rA, _u);
            float num3 = MathUtils.Cross(_rB, _u);
            float num4 = _invMassA + (_invIA * num2 * num2) + _invMassB + (_invIB * num3 * num3);
            _mass = (num4 != 0f) ? (1f / num4) : 0f;
            if (Frequency > 0f)
            {
                float num5 = num - Length;
                float num6 = (float)Math.PI * 2f * Frequency;
                float num7 = 2f * _mass * DampingRatio * num6;
                float num8 = _mass * num6 * num6;
                float dt = data.step.dt;
                _gamma = dt * (num7 + (dt * num8));
                _gamma = (_gamma != 0f) ? (1f / _gamma) : 0f;
                _bias = num5 * dt * num8 * _gamma;
                num4 += _gamma;
                _mass = (num4 != 0f) ? (1f / num4) : 0f;
            }
            else
            {
                _gamma = 0f;
                _bias = 0f;
            }
            _impulse *= data.step.dtRatio;
            Vector2 vector = _impulse * _u;
            v -= _invMassA * vector;
            w -= _invIA * MathUtils.Cross(_rA, vector);
            v2 += _invMassB * vector;
            w2 += _invIB * MathUtils.Cross(_rB, vector);
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
            Vector2 vector = v + MathUtils.Cross(w, _rA);
            Vector2 vector2 = v2 + MathUtils.Cross(w2, _rB);
            float num = Vector2.Dot(_u, vector2 - vector);
            float num2 = (0f - _mass) * (num + _bias + (_gamma * _impulse));
            _impulse += num2;
            Vector2 vector3 = num2 * _u;
            v -= _invMassA * vector3;
            w -= _invIA * MathUtils.Cross(_rA, vector3);
            v2 += _invMassB * vector3;
            w2 += _invIB * MathUtils.Cross(_rB, vector3);
            data.velocities[_indexA].v = v;
            data.velocities[_indexA].w = w;
            data.velocities[_indexB].v = v2;
            data.velocities[_indexB].w = w2;
        }

        internal override bool SolvePositionConstraints(ref SolverData data)
        {
            if (Frequency > 0f)
            {
                return true;
            }
            Vector2 c = data.positions[_indexA].c;
            float a = data.positions[_indexA].a;
            Vector2 c2 = data.positions[_indexB].c;
            float a2 = data.positions[_indexB].a;
            Rot q = new(a);
            Rot q2 = new(a2);
            Vector2 vector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
            Vector2 vector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
            Vector2 vector3 = c2 + vector2 - c - vector;
            float num = vector3.Length();
            vector3 = XnaMath.Normalize(vector3);
            float a3 = num - Length;
            a3 = MathUtils.Clamp(a3, -0.2f, 0.2f);
            float num2 = (0f - _mass) * a3;
            Vector2 vector4 = num2 * vector3;
            c -= _invMassA * vector4;
            a -= _invIA * MathUtils.Cross(vector, vector4);
            c2 += _invMassB * vector4;
            a2 += _invIB * MathUtils.Cross(vector2, vector4);
            data.positions[_indexA].c = c;
            data.positions[_indexA].a = a;
            data.positions[_indexB].c = c2;
            data.positions[_indexB].a = a2;
            return Math.Abs(a3) < 0.005f;
        }
    }
}
