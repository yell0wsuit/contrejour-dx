using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public class TOIInput
{
    public DistanceProxy ProxyA { get; set; } = new();

    public DistanceProxy ProxyB { get; set; } = new();

    public Sweep SweepA { get; set; }

    public Sweep SweepB { get; set; }

    public float TMax { get; set; }
}
