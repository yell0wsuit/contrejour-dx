using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class RopeJoint : Joint
{
	private float _impulse;

	private float _length;

	private int _indexA;

	private int _indexB;

	private Vector2 _localCenterA;

	private Vector2 _localCenterB;

	private float _invMassA;

	private float _invMassB;

	private float _invIA;

	private float _invIB;

	private float _mass;

	private Vector2 _rA;

	private Vector2 _rB;

	private Vector2 _u;

	public Vector2 LocalAnchorA { get; set; }

	public Vector2 LocalAnchorB { get; set; }

	public sealed override Vector2 WorldAnchorA
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

	public sealed override Vector2 WorldAnchorB
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

	public float MaxLength { get; set; }

	public LimitState State { get; private set; }

	internal RopeJoint()
	{
		base.JointType = JointType.Rope;
	}

	public RopeJoint(Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		base.JointType = JointType.Rope;
		if (useWorldCoordinates)
		{
			LocalAnchorA = bodyA.GetLocalPoint(anchorA);
			LocalAnchorB = bodyB.GetLocalPoint(anchorB);
		}
		else
		{
			LocalAnchorA = anchorA;
			LocalAnchorB = anchorB;
		}
		MaxLength = (WorldAnchorB - WorldAnchorA).Length();
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
		_u = c2 + _rB - c - _rA;
		_length = _u.Length();
		float num = _length - MaxLength;
		if (num > 0f)
		{
			State = LimitState.AtUpper;
		}
		else
		{
			State = LimitState.Inactive;
		}
		if (_length > 0.005f)
		{
			_u *= 1f / _length;
			float num2 = MathUtils.Cross(_rA, _u);
			float num3 = MathUtils.Cross(_rB, _u);
			float num4 = _invMassA + _invIA * num2 * num2 + _invMassB + _invIB * num3 * num3;
			_mass = ((num4 != 0f) ? (1f / num4) : 0f);
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
		else
		{
			_u = Vector2.Zero;
			_mass = 0f;
			_impulse = 0f;
		}
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		Vector2 v = data.velocities[_indexA].v;
		float w = data.velocities[_indexA].w;
		Vector2 v2 = data.velocities[_indexB].v;
		float w2 = data.velocities[_indexB].w;
		Vector2 vector = v + MathUtils.Cross(w, _rA);
		Vector2 vector2 = v2 + MathUtils.Cross(w2, _rB);
		float num = _length - MaxLength;
		float num2 = Vector2.Dot(_u, vector2 - vector);
		if (num < 0f)
		{
			num2 += data.step.inv_dt * num;
		}
		float num3 = (0f - _mass) * num2;
		float impulse = _impulse;
		_impulse = Math.Min(0f, _impulse + num3);
		num3 = _impulse - impulse;
		Vector2 vector3 = num3 * _u;
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
		Vector2 c = data.positions[_indexA].c;
		float a = data.positions[_indexA].a;
		Vector2 c2 = data.positions[_indexB].c;
		float a2 = data.positions[_indexB].a;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		Vector2 vector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		Vector2 vector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		Vector2 vector3 = c2 + vector2 - c - vector;
		float num = vector3.Length();
		vector3.Normalize();
		float a3 = num - MaxLength;
		a3 = MathUtils.Clamp(a3, 0f, 0.2f);
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
		return num - MaxLength < 0.005f;
	}
}
