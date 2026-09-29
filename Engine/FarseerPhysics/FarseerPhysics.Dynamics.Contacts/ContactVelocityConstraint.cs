using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

public sealed class ContactVelocityConstraint
{
    public VelocityConstraintPoint[] Points { get; set; } = new VelocityConstraintPoint[2];

    public Vector2 Normal { get; set; }

    public Mat22 normalMass;

    public Mat22 K;

    public int IndexA { get; set; }

    public int IndexB { get; set; }

    public float InvMassA { get; set; }

    public float InvMassB { get; set; }

    public float InvIA { get; set; }

    public float InvIB { get; set; }

    public float Friction { get; set; }

    public float Restitution { get; set; }

    public float TangentSpeed { get; set; }

    public int PointCount { get; set; }

    public int ContactIndex { get; set; }

    public ContactVelocityConstraint()
    {
        for (int i = 0; i < 2; i++)
        {
            Points[i] = new VelocityConstraintPoint();
        }
    }
}
