using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public class DistanceInput
{
    public DistanceProxy ProxyA { get; set; } = new();

    public DistanceProxy ProxyB { get; set; } = new();

    private Transform transformA;

    public ref Transform TransformA => ref transformA;

    private Transform transformB;

    public ref Transform TransformB => ref transformB;

    public bool UseRadii { get; set; }
}
