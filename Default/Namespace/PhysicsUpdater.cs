using FarseerPhysics.Dynamics;

namespace Default.Namespace;

public class PhysicsUpdater : Updatable
{
    public World World { get; }

    public PhysicsUpdater(World world)
    {
        World = world;
        // The listener subscribes itself to the world's contact events.
        _ = new ContactListener(World);
    }

    public override void Update(float time)
    {
        foreach (Body body in World.BodyList)
        {
            if (body.UserData is BodyClip bodyClip)
            {
                bodyClip.Update(time);
            }
        }
    }
}
