using FarseerPhysics.Dynamics;

using Mokus2D.Integration.Farseer.Config;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Physics;

public class PhysicsUpdater : PhysicsTransform, IUpdatable
{
    private readonly World world;

    private readonly FarseerConfig config;

    public World World => world;

    public FarseerConfig Config => config;

    public new float PhysicsToPixels => config.PhysicsToPixels;

    public PhysicsUpdater(World world, FarseerConfig config = null)
        : base(0f)
    {
        config ??= FarseerConfig.DefaultConfig;
        base.PhysicsToPixels = config.PhysicsToPixels;
        this.world = world;
        // The listener subscribes itself to the world's contact events.
        _ = new ContactListener(this.world);
        this.config = config;
    }

    public void Update(float time)
    {
        world.Step(time);
        foreach (Body body in world.BodyList)
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
