using System;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class AngleJoint : Joint
{
	private float _bias;

	private float _jointError;

	private float _massFactor;

	private float _targetAngle;

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

	public float TargetAngle
	{
		get
		{
			return _targetAngle;
		}
		set
		{
			if (value != _targetAngle)
			{
				_targetAngle = value;
				WakeBodies();
			}
		}
	}

	public float BiasFactor { get; set; }

	public float MaxImpulse { get; set; }

	public float Softness { get; set; }

	internal AngleJoint()
	{
		base.JointType = JointType.Angle;
	}

	public AngleJoint(Body bodyA, Body bodyB)
		: base(bodyA, bodyB)
	{
		base.JointType = JointType.Angle;
		BiasFactor = 0.2f;
		MaxImpulse = float.MaxValue;
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return Vector2.Zero;
	}

	public override float GetReactionTorque(float invDt)
	{
		return 0f;
	}

	internal override void InitVelocityConstraints(ref SolverData data)
	{
		int islandIndex = base.BodyA.IslandIndex;
		int islandIndex2 = base.BodyB.IslandIndex;
		float a = data.positions[islandIndex].a;
		float a2 = data.positions[islandIndex2].a;
		_jointError = a2 - a - TargetAngle;
		_bias = (0f - BiasFactor) * data.step.inv_dt * _jointError;
		_massFactor = (1f - Softness) / (base.BodyA._invI + base.BodyB._invI);
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		int islandIndex = base.BodyA.IslandIndex;
		int islandIndex2 = base.BodyB.IslandIndex;
		float value = (_bias - data.velocities[islandIndex2].w + data.velocities[islandIndex].w) * _massFactor;
		data.velocities[islandIndex].w -= base.BodyA._invI * (float)Math.Sign(value) * Math.Min(Math.Abs(value), MaxImpulse);
		data.velocities[islandIndex2].w += base.BodyB._invI * (float)Math.Sign(value) * Math.Min(Math.Abs(value), MaxImpulse);
	}

	internal override bool SolvePositionConstraints(ref SolverData data)
	{
		return true;
	}
}
