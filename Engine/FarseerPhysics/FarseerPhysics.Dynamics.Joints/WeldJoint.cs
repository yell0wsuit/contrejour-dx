using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class WeldJoint : Joint
{
	private Vector3 _impulse;

	private float _gamma;

	private float _bias;

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

	public float ReferenceAngle { get; set; }

	public float FrequencyHz { get; set; }

	public float DampingRatio { get; set; }

	internal WeldJoint()
	{
		base.JointType = JointType.Weld;
	}

	public WeldJoint(Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		base.JointType = JointType.Weld;
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
		ReferenceAngle = base.BodyB.Rotation - base.BodyA.Rotation;
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return invDt * new Vector2(_impulse.X, _impulse.Y);
	}

	public override float GetReactionTorque(float invDt)
	{
		return invDt * _impulse.Z;
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
		float a = data.positions[_indexA].a;
		Vector2 v = data.velocities[_indexA].v;
		float w = data.velocities[_indexA].w;
		float a2 = data.positions[_indexB].a;
		Vector2 v2 = data.velocities[_indexB].v;
		float w2 = data.velocities[_indexB].w;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		_rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		_rB = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		Mat33 mat = default(Mat33);
		mat.ex.X = invMassA + invMassB + _rA.Y * _rA.Y * invIA + _rB.Y * _rB.Y * invIB;
		mat.ey.X = (0f - _rA.Y) * _rA.X * invIA - _rB.Y * _rB.X * invIB;
		mat.ez.X = (0f - _rA.Y) * invIA - _rB.Y * invIB;
		mat.ex.Y = mat.ey.X;
		mat.ey.Y = invMassA + invMassB + _rA.X * _rA.X * invIA + _rB.X * _rB.X * invIB;
		mat.ez.Y = _rA.X * invIA + _rB.X * invIB;
		mat.ex.Z = mat.ez.X;
		mat.ey.Z = mat.ez.Y;
		mat.ez.Z = invIA + invIB;
		if (FrequencyHz > 0f)
		{
			mat.GetInverse22(ref _mass);
			float num = invIA + invIB;
			float num2 = ((num > 0f) ? (1f / num) : 0f);
			float num3 = a2 - a - ReferenceAngle;
			float num4 = (float)Math.PI * 2f * FrequencyHz;
			float num5 = 2f * num2 * DampingRatio * num4;
			float num6 = num2 * num4 * num4;
			float dt = data.step.dt;
			_gamma = dt * (num5 + dt * num6);
			_gamma = ((_gamma != 0f) ? (1f / _gamma) : 0f);
			_bias = num3 * dt * num6 * _gamma;
			num += _gamma;
			_mass.ez.Z = ((num != 0f) ? (1f / num) : 0f);
		}
		else
		{
			mat.GetSymInverse33(ref _mass);
			_gamma = 0f;
			_bias = 0f;
		}
		_impulse *= data.step.dtRatio;
		Vector2 vector = new Vector2(_impulse.X, _impulse.Y);
		v -= invMassA * vector;
		w -= invIA * (MathUtils.Cross(_rA, vector) + _impulse.Z);
		v2 += invMassB * vector;
		w2 += invIB * (MathUtils.Cross(_rB, vector) + _impulse.Z);
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
		if (FrequencyHz > 0f)
		{
			float num = w2 - w;
			float num2 = (0f - _mass.ez.Z) * (num + _bias + _gamma * _impulse.Z);
			_impulse.Z += num2;
			w -= invIA * num2;
			w2 += invIB * num2;
			Vector2 v3 = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA);
			Vector2 vector = -MathUtils.Mul22(_mass, v3);
			_impulse.X += vector.X;
			_impulse.Y += vector.Y;
			Vector2 vector2 = vector;
			v -= invMassA * vector2;
			w -= invIA * MathUtils.Cross(_rA, vector2);
			v2 += invMassB * vector2;
			w2 += invIB * MathUtils.Cross(_rB, vector2);
		}
		else
		{
			Vector2 vector3 = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA);
			float z = w2 - w;
			Vector3 vector4 = -MathUtils.Mul(v: new Vector3(vector3.X, vector3.Y, z), A: _mass);
			_impulse += vector4;
			Vector2 vector5 = new Vector2(vector4.X, vector4.Y);
			v -= invMassA * vector5;
			w -= invIA * (MathUtils.Cross(_rA, vector5) + vector4.Z);
			v2 += invMassB * vector5;
			w2 += invIB * (MathUtils.Cross(_rB, vector5) + vector4.Z);
		}
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
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		Vector2 vector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		Vector2 vector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		Mat33 mat = default(Mat33);
		mat.ex.X = invMassA + invMassB + vector.Y * vector.Y * invIA + vector2.Y * vector2.Y * invIB;
		mat.ey.X = (0f - vector.Y) * vector.X * invIA - vector2.Y * vector2.X * invIB;
		mat.ez.X = (0f - vector.Y) * invIA - vector2.Y * invIB;
		mat.ex.Y = mat.ey.X;
		mat.ey.Y = invMassA + invMassB + vector.X * vector.X * invIA + vector2.X * vector2.X * invIB;
		mat.ez.Y = vector.X * invIA + vector2.X * invIB;
		mat.ex.Z = mat.ez.X;
		mat.ey.Z = mat.ez.Y;
		mat.ez.Z = invIA + invIB;
		float num;
		float num2;
		if (FrequencyHz > 0f)
		{
			Vector2 b = c2 + vector2 - c - vector;
			num = b.Length();
			num2 = 0f;
			Vector2 vector3 = -mat.Solve22(b);
			c -= invMassA * vector3;
			a -= invIA * MathUtils.Cross(vector, vector3);
			c2 += invMassB * vector3;
			a2 += invIB * MathUtils.Cross(vector2, vector3);
		}
		else
		{
			Vector2 vector4 = c2 + vector2 - c - vector;
			float num3 = a2 - a - ReferenceAngle;
			num = vector4.Length();
			num2 = Math.Abs(num3);
			Vector3 b2 = new Vector3(vector4.X, vector4.Y, num3);
			Vector3 vector5 = -mat.Solve33(b2);
			Vector2 vector6 = new Vector2(vector5.X, vector5.Y);
			c -= invMassA * vector6;
			a -= invIA * (MathUtils.Cross(vector, vector6) + vector5.Z);
			c2 += invMassB * vector6;
			a2 += invIB * (MathUtils.Cross(vector2, vector6) + vector5.Z);
		}
		data.positions[_indexA].c = c;
		data.positions[_indexA].a = a;
		data.positions[_indexB].c = c2;
		data.positions[_indexB].a = a2;
		if (num <= 0.005f)
		{
			return num2 <= (float)Math.PI / 90f;
		}
		return false;
	}
}
