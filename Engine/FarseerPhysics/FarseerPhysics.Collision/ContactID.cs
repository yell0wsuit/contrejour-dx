using System.Runtime.InteropServices;

namespace FarseerPhysics.Collision;

[StructLayout(LayoutKind.Explicit)]
public struct ContactID
{
	[FieldOffset(0)]
	public ContactFeature Features;

	[FieldOffset(0)]
	public uint Key;
}
