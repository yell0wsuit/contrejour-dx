using FarseerPhysics.Dynamics;

namespace Default.Namespace;

public class PhysicsUpdater : Updatable
{
    private World world;

    public World World => world;

    public PhysicsUpdater(World world)
    {
        this.world = world;
        // The listener subscribes itself to the world's contact events.
        _ = new ContactListener(this.world);
    }

    public override void Update(float time)
    {
        foreach (Body body in world.BodyList)
        {
            if (body.UserData is BodyClip bodyClip)
            {
                bodyClip.Update(time);
            }
        }
    }
}
