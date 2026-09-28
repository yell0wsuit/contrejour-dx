using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Controllers;

public abstract class AbstractForceController : Controller
{
	public enum DecayModes
	{
		None,
		Step,
		Linear,
		InverseSquare,
		Curve
	}

	public enum ForceTypes
	{
		Point,
		Line,
		Area
	}

	public enum TimingModes
	{
		Switched,
		Triggered,
		Curve
	}

	public Curve DecayCurve;

	public ForceTypes ForceType;

	protected Random Randomize;

	public Curve StrengthCurve;

	public float Strength { get; set; }

	public Vector2 Position { get; set; }

	public float MaximumSpeed { get; set; }

	public float MaximumForce { get; set; }

	public TimingModes TimingMode { get; set; }

	public float ImpulseTime { get; private set; }

	public float ImpulseLength { get; set; }

	public bool Triggered { get; private set; }

	public float Variation { get; set; }

	public DecayModes DecayMode { get; set; }

	public float DecayStart { get; set; }

	public float DecayEnd { get; set; }

	public AbstractForceController()
		: base(ControllerType.AbstractForceController)
	{
		Enabled = true;
		Strength = 1f;
		Position = new Vector2(0f, 0f);
		MaximumSpeed = 100f;
		TimingMode = TimingModes.Switched;
		ImpulseTime = 0f;
		ImpulseLength = 1f;
		Triggered = false;
		StrengthCurve = new Curve();
		Variation = 0f;
		Randomize = new Random(1234);
		DecayMode = DecayModes.None;
		DecayCurve = new Curve();
		DecayStart = 0f;
		DecayEnd = 0f;
		StrengthCurve.Keys.Add(new CurveKey(0f, 5f));
		StrengthCurve.Keys.Add(new CurveKey(0.1f, 5f));
		StrengthCurve.Keys.Add(new CurveKey(0.2f, -4f));
		StrengthCurve.Keys.Add(new CurveKey(1f, 0f));
	}

	public AbstractForceController(TimingModes mode)
		: base(ControllerType.AbstractForceController)
	{
		TimingMode = mode;
		switch (mode)
		{
		case TimingModes.Switched:
			Enabled = true;
			break;
		case TimingModes.Triggered:
			Enabled = false;
			break;
		case TimingModes.Curve:
			Enabled = false;
			break;
		}
	}

	protected float GetDecayMultiplier(Body body)
	{
		float num = (body.Position - Position).Length();
		switch (DecayMode)
		{
		case DecayModes.None:
			return 1f;
		case DecayModes.Step:
			if (num < DecayEnd)
			{
				return 1f;
			}
			return 0f;
		case DecayModes.Linear:
			if (num < DecayStart)
			{
				return 1f;
			}
			if (num > DecayEnd)
			{
				return 0f;
			}
			return DecayEnd - DecayStart / num - DecayStart;
		case DecayModes.InverseSquare:
			if (num < DecayStart)
			{
				return 1f;
			}
			return 1f / ((num - DecayStart) * (num - DecayStart));
		case DecayModes.Curve:
			if (num < DecayStart)
			{
				return 1f;
			}
			return DecayCurve.Evaluate(num - DecayStart);
		default:
			return 1f;
		}
	}

	public void Trigger()
	{
		Triggered = true;
		ImpulseTime = 0f;
	}

	public override void Update(float dt)
	{
		switch (TimingMode)
		{
		case TimingModes.Switched:
			if (Enabled)
			{
				ApplyForce(dt, Strength);
			}
			break;
		case TimingModes.Triggered:
			if (Enabled && Triggered)
			{
				if (ImpulseTime < ImpulseLength)
				{
					ApplyForce(dt, Strength);
					ImpulseTime += dt;
				}
				else
				{
					Triggered = false;
				}
			}
			break;
		case TimingModes.Curve:
			if (Enabled && Triggered)
			{
				if (ImpulseTime < ImpulseLength)
				{
					ApplyForce(dt, Strength * StrengthCurve.Evaluate(ImpulseTime));
					ImpulseTime += dt;
				}
				else
				{
					Triggered = false;
				}
			}
			break;
		}
	}

	public abstract void ApplyForce(float dt, float strength);
}
