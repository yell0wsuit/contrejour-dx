using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public class DistanceInput
{
    public DistanceProxy ProxyA { get; set; } = new();

    public DistanceProxy ProxyB { get; set; } = new();

    public Transform TransformA;

    public Transform TransformB;

    public bool UseRadii { get; set; }
}
