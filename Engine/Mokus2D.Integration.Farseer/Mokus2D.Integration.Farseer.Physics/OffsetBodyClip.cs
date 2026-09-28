using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Physics;

public class OffsetBodyClip(PhysicsUpdater updater, Body body, Node clip) : BodyClip(updater, body, clip)
{
    private float RotationOffset;

    private Vector2 PositionOffset;

    public override void UpdatePosition(float time)
    {
        Clip.Position = Updater.ToPixels(Body.Position) + PositionOffset;
    }

    public override void UpdateRotation(float time)
    {
        Clip.RotationRadians = Body.Rotation + RotationOffset;
    }
}
