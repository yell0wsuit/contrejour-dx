using System.Numerics;

using FarseerPhysics.Common;

namespace FarseerPhysics.Dynamics.Joints
{
    public class GearJoint : Joint
    {
        private readonly JointType _typeA;

        private readonly JointType _typeB;

        private readonly Body _bodyA;

        private readonly Body _bodyB;

        private readonly Body _bodyC;

        private readonly Body _bodyD;

        private Vector2 _localAnchorA;

        private Vector2 _localAnchorB;

        private Vector2 _localAnchorC;

        private Vector2 _localAnchorD;

        private Vector2 _localAxisC;

        private Vector2 _localAxisD;

        private readonly float _referenceAngleA;

        private readonly float _referenceAngleB;

        private readonly float _constant;
        private float _impulse;

        private int _indexA;

        private int _indexB;

        private int _indexC;

        private int _indexD;

        private Vector2 _lcA;

        private Vector2 _lcB;

        private Vector2 _lcC;

        private Vector2 _lcD;

        private float _mA;

        private float _mB;

        private float _mC;

        private float _mD;

        private float _iA;

        private float _iB;

        private float _iC;

        private float _iD;

        private Vector2 _JvAC;

        private Vector2 _JvBD;

        private float _JwA;

        private float _JwB;

        private float _JwC;

        private float _JwD;

        private float _mass;

        public override Vector2 WorldAnchorA
        {
            get => _bodyA.GetWorldPoint(_localAnchorA);
            set
            {
            }
        }

        public override Vector2 WorldAnchorB
        {
            get => _bodyB.GetWorldPoint(_localAnchorB);
            set
            {
            }
        }

        public float Ratio { get; set; }

        public Joint JointA { get; private set; }

        public Joint JointB { get; private set; }

        public GearJoint(Body bodyA, Body bodyB, Joint jointA, Joint jointB, float ratio = 1f)
        {
            JointType = JointType.Gear;
            BodyA = bodyA;
            BodyB = bodyB;
            JointA = jointA;
            JointB = jointB;
            Ratio = ratio;
            _typeA = jointA.JointType;
            _typeB = jointB.JointType;
            _bodyC = JointA.BodyA;
            _bodyA = JointA.BodyB;
            Transform xf = _bodyA._xf;
            float a = _bodyA._sweep.A;
            Transform xf2 = _bodyC._xf;
            float a2 = _bodyC._sweep.A;
            float num;
            if (_typeA == JointType.Revolute)
            {
                RevoluteJoint revoluteJoint = (RevoluteJoint)jointA;
                _localAnchorC = revoluteJoint.LocalAnchorA;
                _localAnchorA = revoluteJoint.LocalAnchorB;
                _referenceAngleA = revoluteJoint.ReferenceAngle;
                _localAxisC = Vector2.Zero;
                num = a - a2 - _referenceAngleA;
            }
            else
            {
                PrismaticJoint prismaticJoint = (PrismaticJoint)jointA;
                _localAnchorC = prismaticJoint.LocalAnchorA;
                _localAnchorA = prismaticJoint.LocalAnchorB;
                _referenceAngleA = prismaticJoint.ReferenceAngle;
                _localAxisC = prismaticJoint.LocalXAxis;
                Vector2 localAnchorC = _localAnchorC;
                Vector2 vector = MathUtils.MulT(xf2.q, MathUtils.Mul(xf.q, _localAnchorA) + (xf.p - xf2.p));
                num = Vector2.Dot(vector - localAnchorC, _localAxisC);
            }
            _bodyD = JointB.BodyA;
            _bodyB = JointB.BodyB;
            Transform xf3 = _bodyB._xf;
            float a3 = _bodyB._sweep.A;
            Transform xf4 = _bodyD._xf;
            float a4 = _bodyD._sweep.A;
            float num2;
            if (_typeB == JointType.Revolute)
            {
                RevoluteJoint revoluteJoint2 = (RevoluteJoint)jointB;
                _localAnchorD = revoluteJoint2.LocalAnchorA;
                _localAnchorB = revoluteJoint2.LocalAnchorB;
                _referenceAngleB = revoluteJoint2.ReferenceAngle;
                _localAxisD = Vector2.Zero;
                num2 = a3 - a4 - _referenceAngleB;
            }
            else
            {
                PrismaticJoint prismaticJoint2 = (PrismaticJoint)jointB;
                _localAnchorD = prismaticJoint2.LocalAnchorA;
                _localAnchorB = prismaticJoint2.LocalAnchorB;
                _referenceAngleB = prismaticJoint2.ReferenceAngle;
                _localAxisD = prismaticJoint2.LocalXAxis;
                Vector2 localAnchorD = _localAnchorD;
                Vector2 vector2 = MathUtils.MulT(xf4.q, MathUtils.Mul(xf3.q, _localAnchorB) + (xf3.p - xf4.p));
                num2 = Vector2.Dot(vector2 - localAnchorD, _localAxisD);
            }
            Ratio = ratio;
            _constant = num + (Ratio * num2);
            _impulse = 0f;
        }

        public override Vector2 GetReactionForce(float invDt)
        {
            Vector2 vector = _impulse * _JvAC;
            return invDt * vector;
        }

        public override float GetReactionTorque(float invDt)
        {
            float num = _impulse * _JwA;
            return invDt * num;
        }

        internal override void InitVelocityConstraints(ref SolverData data)
        {
            _indexA = _bodyA.IslandIndex;
            _indexB = _bodyB.IslandIndex;
            _indexC = _bodyC.IslandIndex;
            _indexD = _bodyD.IslandIndex;
            _lcA = _bodyA._sweep.LocalCenter;
            _lcB = _bodyB._sweep.LocalCenter;
            _lcC = _bodyC._sweep.LocalCenter;
            _lcD = _bodyD._sweep.LocalCenter;
            _mA = _bodyA._invMass;
            _mB = _bodyB._invMass;
            _mC = _bodyC._invMass;
            _mD = _bodyD._invMass;
            _iA = _bodyA._invI;
            _iB = _bodyB._invI;
            _iC = _bodyC._invI;
            _iD = _bodyD._invI;
            float a = data.positions[_indexA].a;
            Vector2 v = data.velocities[_indexA].v;
            float w = data.velocities[_indexA].w;
            float a2 = data.positions[_indexB].a;
            Vector2 v2 = data.velocities[_indexB].v;
            float w2 = data.velocities[_indexB].w;
            float a3 = data.positions[_indexC].a;
            Vector2 v3 = data.velocities[_indexC].v;
            float w3 = data.velocities[_indexC].w;
            float a4 = data.positions[_indexD].a;
            Vector2 v4 = data.velocities[_indexD].v;
            float w4 = data.velocities[_indexD].w;
            Rot q = new(a);
            Rot q2 = new(a2);
            Rot q3 = new(a3);
            Rot q4 = new(a4);
            _mass = 0f;
            if (_typeA == JointType.Revolute)
            {
                _JvAC = Vector2.Zero;
                _JwA = 1f;
                _JwC = 1f;
                _mass += _iA + _iC;
            }
            else
            {
                Vector2 vector = MathUtils.Mul(q3, _localAxisC);
                Vector2 a5 = MathUtils.Mul(q3, _localAnchorC - _lcC);
                Vector2 a6 = MathUtils.Mul(q, _localAnchorA - _lcA);
                _JvAC = vector;
                _JwC = MathUtils.Cross(a5, vector);
                _JwA = MathUtils.Cross(a6, vector);
                _mass += _mC + _mA + (_iC * _JwC * _JwC) + (_iA * _JwA * _JwA);
            }
            if (_typeB == JointType.Revolute)
            {
                _JvBD = Vector2.Zero;
                _JwB = Ratio;
                _JwD = Ratio;
                _mass += Ratio * Ratio * (_iB + _iD);
            }
            else
            {
                Vector2 vector2 = MathUtils.Mul(q4, _localAxisD);
                Vector2 a7 = MathUtils.Mul(q4, _localAnchorD - _lcD);
                Vector2 a8 = MathUtils.Mul(q2, _localAnchorB - _lcB);
                _JvBD = Ratio * vector2;
                _JwD = Ratio * MathUtils.Cross(a7, vector2);
                _JwB = Ratio * MathUtils.Cross(a8, vector2);
                _mass += (Ratio * Ratio * (_mD + _mB)) + (_iD * _JwD * _JwD) + (_iB * _JwB * _JwB);
            }
            _mass = (_mass > 0f) ? (1f / _mass) : 0f;
            v += _mA * _impulse * _JvAC;
            w += _iA * _impulse * _JwA;
            v2 += _mB * _impulse * _JvBD;
            w2 += _iB * _impulse * _JwB;
            v3 -= _mC * _impulse * _JvAC;
            w3 -= _iC * _impulse * _JwC;
            v4 -= _mD * _impulse * _JvBD;
            w4 -= _iD * _impulse * _JwD;
            data.velocities[_indexA].v = v;
            data.velocities[_indexA].w = w;
            data.velocities[_indexB].v = v2;
            data.velocities[_indexB].w = w2;
            data.velocities[_indexC].v = v3;
            data.velocities[_indexC].w = w3;
            data.velocities[_indexD].v = v4;
            data.velocities[_indexD].w = w4;
        }

        internal override void SolveVelocityConstraints(ref SolverData data)
        {
            Vector2 v = data.velocities[_indexA].v;
            float w = data.velocities[_indexA].w;
            Vector2 v2 = data.velocities[_indexB].v;
            float w2 = data.velocities[_indexB].w;
            Vector2 v3 = data.velocities[_indexC].v;
            float w3 = data.velocities[_indexC].w;
            Vector2 v4 = data.velocities[_indexD].v;
            float w4 = data.velocities[_indexD].w;
            float num = Vector2.Dot(_JvAC, v - v3) + Vector2.Dot(_JvBD, v2 - v4);
            num += (_JwA * w) - (_JwC * w3) + ((_JwB * w2) - (_JwD * w4));
            float num2 = (0f - _mass) * num;
            _impulse += num2;
            v += _mA * num2 * _JvAC;
            w += _iA * num2 * _JwA;
            v2 += _mB * num2 * _JvBD;
            w2 += _iB * num2 * _JwB;
            v3 -= _mC * num2 * _JvAC;
            w3 -= _iC * num2 * _JwC;
            v4 -= _mD * num2 * _JvBD;
            w4 -= _iD * num2 * _JwD;
            data.velocities[_indexA].v = v;
            data.velocities[_indexA].w = w;
            data.velocities[_indexB].v = v2;
            data.velocities[_indexB].w = w2;
            data.velocities[_indexC].v = v3;
            data.velocities[_indexC].w = w3;
            data.velocities[_indexD].v = v4;
            data.velocities[_indexD].w = w4;
        }

        internal override bool SolvePositionConstraints(ref SolverData data)
        {
            Vector2 c = data.positions[_indexA].c;
            float a = data.positions[_indexA].a;
            Vector2 c2 = data.positions[_indexB].c;
            float a2 = data.positions[_indexB].a;
            Vector2 c3 = data.positions[_indexC].c;
            float a3 = data.positions[_indexC].a;
            Vector2 c4 = data.positions[_indexD].c;
            float a4 = data.positions[_indexD].a;
            Rot q = new(a);
            Rot q2 = new(a2);
            Rot q3 = new(a3);
            Rot q4 = new(a4);
            float num = 0f;
            Vector2 vector;
            float num2;
            float num3;
            float num4;
            if (_typeA == JointType.Revolute)
            {
                vector = Vector2.Zero;
                num2 = 1f;
                num3 = 1f;
                num += _iA + _iC;
                num4 = a - a3 - _referenceAngleA;
            }
            else
            {
                Vector2 vector2 = MathUtils.Mul(q3, _localAxisC);
                Vector2 a5 = MathUtils.Mul(q3, _localAnchorC - _lcC);
                Vector2 vector3 = MathUtils.Mul(q, _localAnchorA - _lcA);
                vector = vector2;
                num3 = MathUtils.Cross(a5, vector2);
                num2 = MathUtils.Cross(vector3, vector2);
                num += _mC + _mA + (_iC * num3 * num3) + (_iA * num2 * num2);
                Vector2 vector4 = _localAnchorC - _lcC;
                Vector2 vector5 = MathUtils.MulT(q3, vector3 + (c - c3));
                num4 = Vector2.Dot(vector5 - vector4, _localAxisC);
            }
            Vector2 vector6;
            float num5;
            float num6;
            float num7;
            if (_typeB == JointType.Revolute)
            {
                vector6 = Vector2.Zero;
                num5 = Ratio;
                num6 = Ratio;
                num += Ratio * Ratio * (_iB + _iD);
                num7 = a2 - a4 - _referenceAngleB;
            }
            else
            {
                Vector2 vector7 = MathUtils.Mul(q4, _localAxisD);
                Vector2 a6 = MathUtils.Mul(q4, _localAnchorD - _lcD);
                Vector2 vector8 = MathUtils.Mul(q2, _localAnchorB - _lcB);
                vector6 = Ratio * vector7;
                num6 = Ratio * MathUtils.Cross(a6, vector7);
                num5 = Ratio * MathUtils.Cross(vector8, vector7);
                num += (Ratio * Ratio * (_mD + _mB)) + (_iD * num6 * num6) + (_iB * num5 * num5);
                Vector2 vector9 = _localAnchorD - _lcD;
                Vector2 vector10 = MathUtils.MulT(q4, vector8 + (c2 - c4));
                num7 = Vector2.Dot(vector10 - vector9, _localAxisD);
            }
            float num8 = num4 + (Ratio * num7) - _constant;
            float num9 = 0f;
            if (num > 0f)
            {
                num9 = (0f - num8) / num;
            }
            c += _mA * num9 * vector;
            a += _iA * num9 * num2;
            c2 += _mB * num9 * vector6;
            a2 += _iB * num9 * num5;
            c3 -= _mC * num9 * vector;
            a3 -= _iC * num9 * num3;
            c4 -= _mD * num9 * vector6;
            a4 -= _iD * num9 * num6;
            data.positions[_indexA].c = c;
            data.positions[_indexA].a = a;
            data.positions[_indexB].c = c2;
            data.positions[_indexB].a = a2;
            data.positions[_indexC].c = c3;
            data.positions[_indexC].a = a3;
            data.positions[_indexD].c = c4;
            data.positions[_indexD].a = a4;
            return true;
        }
    }
}
