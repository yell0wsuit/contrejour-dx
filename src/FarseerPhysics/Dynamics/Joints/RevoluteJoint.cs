using System;
using System.Numerics;

using FarseerPhysics.Common;

namespace FarseerPhysics.Dynamics.Joints
{
    public class RevoluteJoint : Joint
    {
        private Vector3 _impulse;

        private float _motorImpulse;
        private float _lowerAngle;

        private float _upperAngle;

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

        private Mat33 _mass;

        private float _motorMass;

        private LimitState _limitState;

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

        public float ReferenceAngle
        {
            get;
            set
            {
                WakeBodies();
                field = value;
            }
        }

        public float JointAngle => BodyB._sweep.A - BodyA._sweep.A - ReferenceAngle;

        public float JointSpeed => BodyB._angularVelocity - BodyA._angularVelocity;

        public bool LimitEnabled
        {
            get;
            set
            {
                if (field != value)
                {
                    WakeBodies();
                    field = value;
                    _impulse.Z = 0f;
                }
            }
        }

        public float LowerLimit
        {
            get => _lowerAngle;
            set
            {
                if (_lowerAngle != value)
                {
                    WakeBodies();
                    _lowerAngle = value;
                    _impulse.Z = 0f;
                }
            }
        }

        public float UpperLimit
        {
            get => _upperAngle;
            set
            {
                if (_upperAngle != value)
                {
                    WakeBodies();
                    _upperAngle = value;
                    _impulse.Z = 0f;
                }
            }
        }

        public bool MotorEnabled
        {
            get;
            set
            {
                WakeBodies();
                field = value;
            }
        }

        public float MotorSpeed
        {
            get;
            set
            {
                WakeBodies();
                field = value;
            }
        }

        public float MaxMotorTorque
        {
            get;
            set
            {
                WakeBodies();
                field = value;
            }
        }

        public float MotorImpulse
        {
            get => _motorImpulse;
            set
            {
                WakeBodies();
                _motorImpulse = value;
            }
        }

        internal RevoluteJoint()
        {
            JointType = JointType.Revolute;
        }

        public RevoluteJoint(Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, bool useWorldCoordinates = false)
            : base(bodyA, bodyB)
        {
            JointType = JointType.Revolute;
            if (useWorldCoordinates)
            {
                LocalAnchorA = BodyA.GetLocalPoint(anchorA);
                LocalAnchorB = BodyB.GetLocalPoint(anchorB);
            }
            else
            {
                LocalAnchorA = anchorA;
                LocalAnchorB = anchorB;
            }
            ReferenceAngle = BodyB.Rotation - BodyA.Rotation;
            _impulse = Vector3.Zero;
            _limitState = LimitState.Inactive;
        }

        public RevoluteJoint(Body bodyA, Body bodyB, Vector2 anchor, bool useWorldCoordinates = false)
            : this(bodyA, bodyB, anchor, anchor, useWorldCoordinates)
        {
        }

        public void SetLimits(float lower, float upper)
        {
            if (lower != _lowerAngle || upper != _upperAngle)
            {
                WakeBodies();
                _upperAngle = upper;
                _lowerAngle = lower;
                _impulse.Z = 0f;
            }
        }

        public float GetMotorTorque(float invDt)
        {
            return invDt * _motorImpulse;
        }

        public override Vector2 GetReactionForce(float invDt)
        {
            Vector2 vector = new(_impulse.X, _impulse.Y);
            return invDt * vector;
        }

        public override float GetReactionTorque(float invDt)
        {
            return invDt * _impulse.Z;
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
            bool flag = invIA + invIB == 0f;
            _mass.ex.X = invMassA + invMassB + (_rA.Y * _rA.Y * invIA) + (_rB.Y * _rB.Y * invIB);
            _mass.ey.X = ((0f - _rA.Y) * _rA.X * invIA) - (_rB.Y * _rB.X * invIB);
            _mass.ez.X = ((0f - _rA.Y) * invIA) - (_rB.Y * invIB);
            _mass.ex.Y = _mass.ey.X;
            _mass.ey.Y = invMassA + invMassB + (_rA.X * _rA.X * invIA) + (_rB.X * _rB.X * invIB);
            _mass.ez.Y = (_rA.X * invIA) + (_rB.X * invIB);
            _mass.ex.Z = _mass.ez.X;
            _mass.ey.Z = _mass.ez.Y;
            _mass.ez.Z = invIA + invIB;
            _motorMass = invIA + invIB;
            if (_motorMass > 0f)
            {
                _motorMass = 1f / _motorMass;
            }
            if (!MotorEnabled || flag)
            {
                _motorImpulse = 0f;
            }
            if (LimitEnabled && !flag)
            {
                float num = a2 - a - ReferenceAngle;
                if (Math.Abs(_upperAngle - _lowerAngle) < (float)Math.PI / 45f)
                {
                    _limitState = LimitState.Equal;
                }
                else if (num <= _lowerAngle)
                {
                    if (_limitState != LimitState.AtLower)
                    {
                        _impulse.Z = 0f;
                    }
                    _limitState = LimitState.AtLower;
                }
                else if (num >= _upperAngle)
                {
                    if (_limitState != LimitState.AtUpper)
                    {
                        _impulse.Z = 0f;
                    }
                    _limitState = LimitState.AtUpper;
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
            }
            _impulse *= data.step.dtRatio;
            _motorImpulse *= data.step.dtRatio;
            Vector2 vector = new(_impulse.X, _impulse.Y);
            v -= invMassA * vector;
            w -= invIA * (MathUtils.Cross(_rA, vector) + MotorImpulse + _impulse.Z);
            v2 += invMassB * vector;
            w2 += invIB * (MathUtils.Cross(_rB, vector) + MotorImpulse + _impulse.Z);
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
            bool flag = invIA + invIB == 0f;
            if (MotorEnabled && _limitState != LimitState.Equal && !flag)
            {
                float num3 = num2 - num - MotorSpeed;
                float num4 = _motorMass * (0f - num3);
                float motorImpulse = _motorImpulse;
                float num5 = data.step.dt * MaxMotorTorque;
                _motorImpulse = MathUtils.Clamp(_motorImpulse + num4, 0f - num5, num5);
                num4 = _motorImpulse - motorImpulse;
                num -= invIA * num4;
                num2 += invIB * num4;
            }
            if (LimitEnabled && _limitState != LimitState.Inactive && !flag)
            {
                Vector2 vector = v2 + MathUtils.Cross(num2, _rB) - v - MathUtils.Cross(num, _rA);
                float z = num2 - num;
                Vector3 b = new(vector.X, vector.Y, z);
                Vector3 vector2 = -_mass.Solve33(b);
                if (_limitState == LimitState.Equal)
                {
                    _impulse += vector2;
                }
                else if (_limitState == LimitState.AtLower)
                {
                    float num6 = _impulse.Z + vector2.Z;
                    if (num6 < 0f)
                    {
                        Vector2 b2 = -vector + (_impulse.Z * new Vector2(_mass.ez.X, _mass.ez.Y));
                        Vector2 vector3 = _mass.Solve22(b2);
                        vector2.X = vector3.X;
                        vector2.Y = vector3.Y;
                        vector2.Z = 0f - _impulse.Z;
                        _impulse.X += vector3.X;
                        _impulse.Y += vector3.Y;
                        _impulse.Z = 0f;
                    }
                    else
                    {
                        _impulse += vector2;
                    }
                }
                else if (_limitState == LimitState.AtUpper)
                {
                    float num7 = _impulse.Z + vector2.Z;
                    if (num7 > 0f)
                    {
                        Vector2 b3 = -vector + (_impulse.Z * new Vector2(_mass.ez.X, _mass.ez.Y));
                        Vector2 vector4 = _mass.Solve22(b3);
                        vector2.X = vector4.X;
                        vector2.Y = vector4.Y;
                        vector2.Z = 0f - _impulse.Z;
                        _impulse.X += vector4.X;
                        _impulse.Y += vector4.Y;
                        _impulse.Z = 0f;
                    }
                    else
                    {
                        _impulse += vector2;
                    }
                }
                Vector2 vector5 = new(vector2.X, vector2.Y);
                v -= invMassA * vector5;
                num -= invIA * (MathUtils.Cross(_rA, vector5) + vector2.Z);
                v2 += invMassB * vector5;
                num2 += invIB * (MathUtils.Cross(_rB, vector5) + vector2.Z);
            }
            else
            {
                Vector2 vector6 = v2 + MathUtils.Cross(num2, _rB) - v - MathUtils.Cross(num, _rA);
                Vector2 vector7 = _mass.Solve22(-vector6);
                _impulse.X += vector7.X;
                _impulse.Y += vector7.Y;
                v -= invMassA * vector7;
                num -= invIA * MathUtils.Cross(_rA, vector7);
                v2 += invMassB * vector7;
                num2 += invIB * MathUtils.Cross(_rB, vector7);
            }
            data.velocities[_indexA].v = v;
            data.velocities[_indexA].w = num;
            data.velocities[_indexB].v = v2;
            data.velocities[_indexB].w = num2;
        }

        internal override bool SolvePositionConstraints(ref SolverData data)
        {
            Vector2 c = data.positions[_indexA].c;
            float num = data.positions[_indexA].a;
            Vector2 c2 = data.positions[_indexB].c;
            float num2 = data.positions[_indexB].a;
            Rot q = new(num);
            Rot q2 = new(num2);
            float num3 = 0f;
            bool flag = _invIA + _invIB == 0f;
            if (LimitEnabled && _limitState != LimitState.Inactive && !flag)
            {
                float num4 = num2 - num - ReferenceAngle;
                float num5 = 0f;
                if (_limitState == LimitState.Equal)
                {
                    float num6 = MathUtils.Clamp(num4 - _lowerAngle, (float)Math.PI * -2f / 45f, (float)Math.PI * 2f / 45f);
                    num5 = (0f - _motorMass) * num6;
                    num3 = Math.Abs(num6);
                }
                else if (_limitState == LimitState.AtLower)
                {
                    float num7 = num4 - _lowerAngle;
                    num3 = 0f - num7;
                    num7 = MathUtils.Clamp(num7 + ((float)Math.PI / 90f), (float)Math.PI * -2f / 45f, 0f);
                    num5 = (0f - _motorMass) * num7;
                }
                else if (_limitState == LimitState.AtUpper)
                {
                    float num8 = num4 - _upperAngle;
                    num3 = num8;
                    num8 = MathUtils.Clamp(num8 - ((float)Math.PI / 90f), 0f, (float)Math.PI * 2f / 45f);
                    num5 = (0f - _motorMass) * num8;
                }
                num -= _invIA * num5;
                num2 += _invIB * num5;
            }
            q.Set(num);
            q2.Set(num2);
            Vector2 vector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
            Vector2 vector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
            Vector2 b = c2 + vector2 - c - vector;
            float num9 = b.Length();
            float invMassA = _invMassA;
            float invMassB = _invMassB;
            float invIA = _invIA;
            float invIB = _invIB;
            Mat22 mat = default;
            mat.ex.X = invMassA + invMassB + (invIA * vector.Y * vector.Y) + (invIB * vector2.Y * vector2.Y);
            mat.ex.Y = ((0f - invIA) * vector.X * vector.Y) - (invIB * vector2.X * vector2.Y);
            mat.ey.X = mat.ex.Y;
            mat.ey.Y = invMassA + invMassB + (invIA * vector.X * vector.X) + (invIB * vector2.X * vector2.X);
            Vector2 vector3 = -mat.Solve(b);
            c -= invMassA * vector3;
            num -= invIA * MathUtils.Cross(vector, vector3);
            c2 += invMassB * vector3;
            num2 += invIB * MathUtils.Cross(vector2, vector3);
            data.positions[_indexA].c = c;
            data.positions[_indexA].a = num;
            data.positions[_indexB].c = c2;
            data.positions[_indexB].a = num2;
            return num9 <= 0.005f && num3 <= (float)Math.PI / 90f;
        }
    }
}
