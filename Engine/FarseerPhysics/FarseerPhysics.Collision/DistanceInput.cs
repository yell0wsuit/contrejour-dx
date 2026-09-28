using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public class DistanceInput
{
    public DistanceProxy ProxyA = new();

    public DistanceProxy ProxyB = new();

    public Transform TransformA;

    public Transform TransformB;

    public bool UseRadii;
}
