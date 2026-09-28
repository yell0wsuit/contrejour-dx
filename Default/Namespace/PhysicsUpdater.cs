using FarseerPhysics.Dynamics;

namespace Default.Namespace;

public class PhysicsUpdater : Updatable
{
    protected World world;

    protected ContactListener listener;

    public World World => world;

    public PhysicsUpdater(World _world)
    {
        world = _world;
        listener = new ContactListener(world);
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
