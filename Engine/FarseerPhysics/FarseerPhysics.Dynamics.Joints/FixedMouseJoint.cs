using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class FixedMouseJoint : Joint
{
	private Vector2 _worldAnchor;

	private float _frequency;

	private float _dampingRatio;

	private float _beta;

	private Vector2 _impulse;

	private float _maxForce;

	private float _gamma;

	private int _indexA;

	private Vector2 _rA;

	private Vector2 _localCenterA;

	private float _invMassA;

	private float _invIA;

	private Mat22 _mass;

	private Vector2 _C;

	public Vector2 LocalAnchorA { get; set; }

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
			return _worldAnchor;
		}
		set
		{
			WakeBodies();
			_worldAnchor = value;
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

	public float Frequency
	{
		get
		{
			return _frequency;
		}
		set
		{
			_frequency = value;
		}
	}

	public float DampingRatio
	{
		get
		{
			return _dampingRatio;
		}
		set
		{
			_dampingRatio = value;
		}
	}

	public FixedMouseJoint(Body body, Vector2 worldAnchor)
		: base(body)
	{
		base.JointType = JointType.FixedMouse;
		Frequency = 5f;
		DampingRatio = 0.7f;
		MaxForce = 1000f * body.Mass;
		_worldAnchor = worldAnchor;
		LocalAnchorA = MathUtils.MulT(base.BodyA._xf, worldAnchor);
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return invDt * _impulse;
	}

	public override float GetReactionTorque(float invDt)
	{
		return invDt * 0f;
	}

	internal override void InitVelocityConstraints(ref SolverData data)
	{
		_indexA = base.BodyA.IslandIndex;
		_localCenterA = base.BodyA._sweep.LocalCenter;
		_invMassA = base.BodyA._invMass;
		_invIA = base.BodyA._invI;
		Vector2 c = data.positions[_indexA].c;
		float a = data.positions[_indexA].a;
		Vector2 v = data.velocities[_indexA].v;
		float w = data.velocities[_indexA].w;
		Rot q = new Rot(a);
		float mass = base.BodyA.Mass;
		float num = (float)Math.PI * 2f * Frequency;
		float num2 = 2f * mass * DampingRatio * num;
		float num3 = mass * (num * num);
		float dt = data.step.dt;
		_gamma = dt * (num2 + dt * num3);
		if (_gamma != 0f)
		{
			_gamma = 1f / _gamma;
		}
		_beta = dt * num3 * _gamma;
		_rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		Mat22 mat = default(Mat22);
		mat.ex.X = _invMassA + _invIA * _rA.Y * _rA.Y + _gamma;
		mat.ex.Y = (0f - _invIA) * _rA.X * _rA.Y;
		mat.ey.X = mat.ex.Y;
		mat.ey.Y = _invMassA + _invIA * _rA.X * _rA.X + _gamma;
		_mass = mat.Inverse;
		_C = c + _rA - _worldAnchor;
		_C *= _beta;
		w *= 0.98f;
		_impulse *= data.step.dtRatio;
		v += _invMassA * _impulse;
		w += _invIA * MathUtils.Cross(_rA, _impulse);
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		Vector2 v = data.velocities[_indexA].v;
		float w = data.velocities[_indexA].w;
		Vector2 vector = v + MathUtils.Cross(w, _rA);
		Vector2 vector2 = MathUtils.Mul(ref _mass, -(vector + _C + _gamma * _impulse));
		Vector2 impulse = _impulse;
		_impulse += vector2;
		float num = data.step.dt * MaxForce;
		if (_impulse.LengthSquared() > num * num)
		{
			_impulse *= num / _impulse.Length();
		}
		vector2 = _impulse - impulse;
		v += _invMassA * vector2;
		w += _invIA * MathUtils.Cross(_rA, vector2);
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
	}

	internal override bool SolvePositionConstraints(ref SolverData data)
	{
		return true;
	}
}
