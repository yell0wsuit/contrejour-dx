using FarseerPhysics.Dynamics;

using Mokus2D.Integration.Farseer.Config;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Physics;

public class PhysicsUpdater : PhysicsTransform, IUpdatable
{
    public World World { get; }

    public FarseerConfig Config { get; }

    public new float PhysicsToPixels => Config.PhysicsToPixels;

    public PhysicsUpdater(World world, FarseerConfig config = null)
        : base(0f)
    {
        config ??= FarseerConfig.DefaultConfig;
        base.PhysicsToPixels = config.PhysicsToPixels;
        World = world;
        // The listener subscribes itself to the world's contact events.
        _ = new ContactListener(World);
        Config = config;
    }

    public void Update(float time)
    {
        World.Step(time);
        foreach (Body body in World.BodyList)
        {
            if (body.UserData is BodyClip bodyClip)
            {
                bodyClip.Update(time);
            }
        }
    }

    public void ApplyTransformations(Body from, Node to)
    {
        to.Position = ToPixels(from.Position);
        to.RotationRadians = to.RotationRadians;
    }
}
