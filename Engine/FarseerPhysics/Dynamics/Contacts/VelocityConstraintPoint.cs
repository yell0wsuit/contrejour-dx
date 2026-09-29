using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

public sealed class VelocityConstraintPoint
{
    public Vector2 RA { get; set; }

    public Vector2 RB { get; set; }

    public float NormalImpulse { get; set; }

    public float TangentImpulse { get; set; }

    public float NormalMass { get; set; }

    public float TangentMass { get; set; }

    public float VelocityBias { get; set; }
}
