using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public class TOIInput
{
    public DistanceProxy ProxyA = new();

    public DistanceProxy ProxyB = new();

    public Sweep SweepA;

    public Sweep SweepB;

    public float TMax;
}
