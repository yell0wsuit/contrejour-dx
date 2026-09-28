using FarseerPhysics.Dynamics;

namespace Default.Namespace;

public class PhysicsUpdater : Updatable
{
    private World world;

    private ContactListener listener;

    public World World => world;

    public PhysicsUpdater(World world)
    {
        this.world = world;
        listener = new ContactListener(this.world);
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
