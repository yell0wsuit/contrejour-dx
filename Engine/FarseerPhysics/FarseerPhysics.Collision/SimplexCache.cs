using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public struct SimplexCache
{
	public ushort Count;

	public FixedArray3<byte> IndexA;

	public FixedArray3<byte> IndexB;

	public float Metric;
}
