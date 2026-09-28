using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public class TOIInput
{
	public DistanceProxy ProxyA = new DistanceProxy();

	public DistanceProxy ProxyB = new DistanceProxy();

	public Sweep SweepA;

	public Sweep SweepB;

	public float TMax;
}
