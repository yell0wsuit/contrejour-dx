using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Physics;

public class OffsetBodyClip : BodyClip
{
	public float RotationOffset;

	public Vector2 PositionOffset;

	public OffsetBodyClip(PhysicsUpdater updater, Body body, Node clip)
		: base(updater, body, clip)
	{
	}

	public override void UpdatePosition(float time)
	{
		base.Clip.Position = Updater.ToPixels(Body.Position) + PositionOffset;
	}

	public override void UpdateRotation(float time)
	{
		base.Clip.RotationRadians = Body.Rotation + RotationOffset;
	}
}
