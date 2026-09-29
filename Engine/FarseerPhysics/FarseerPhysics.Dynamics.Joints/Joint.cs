using System;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public abstract class Joint
{
    private double _breakpointSquared;

    public bool Enabled { get; set; } = true;

    internal JointEdge EdgeA = new();

    internal JointEdge EdgeB = new();

    internal bool IslandFlag;

    public JointType JointType { get; protected set; }

    public Body BodyA { get; internal set; }

    public Body BodyB { get; internal set; }

    public abstract Vector2 WorldAnchorA { get; set; }

    public abstract Vector2 WorldAnchorB { get; set; }

    public object UserData { get; set; }

    public bool CollideConnected { get; set; }

    public float Breakpoint
    {
        get;
        set
        {
            field = value;
            _breakpointSquared = field * field;
        }
    }

    public event Action<Joint, float> Broke;

    protected Joint()
    {
        Breakpoint = float.MaxValue;
        CollideConnected = false;
    }

    protected Joint(Body bodyA, Body bodyB)
        : this()
    {
        BodyA = bodyA;
        BodyB = bodyB;
    }

    protected Joint(Body body)
        : this()
    {
        BodyA = body;
    }

    public abstract Vector2 GetReactionForce(float invDt);

    public abstract float GetReactionTorque(float invDt);

    protected void WakeBodies()
    {
        BodyA?.Awake = true;
        BodyB?.Awake = true;
    }

    public bool IsFixedType()
    {
        return JointType is JointType.FixedRevolute or JointType.FixedDistance or JointType.FixedPrismatic or JointType.FixedLine or JointType.FixedMouse or JointType.FixedAngle or JointType.FixedFriction;
    }

    internal abstract void InitVelocityConstraints(ref SolverData data);

    internal void Validate(float invDt)
    {
        if (!Enabled)
        {
            return;
        }
        float num = GetReactionForce(invDt).LengthSquared();
        if (!((double)Math.Abs(num) <= _breakpointSquared))
        {
            Enabled = false;
            Broke?.Invoke(this, (float)Math.Sqrt(num));
        }
    }

    internal abstract void SolveVelocityConstraints(ref SolverData data);

    internal abstract bool SolvePositionConstraints(ref SolverData data);
}
