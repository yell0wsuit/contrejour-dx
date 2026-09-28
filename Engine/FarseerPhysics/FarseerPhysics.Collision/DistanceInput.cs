using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public class DistanceInput
{
	public DistanceProxy ProxyA = new DistanceProxy();

	public DistanceProxy ProxyB = new DistanceProxy();

	public Transform TransformA;

	public Transform TransformB;

	public bool UseRadii;
}
