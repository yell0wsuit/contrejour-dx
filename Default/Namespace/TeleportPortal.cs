using Mokus2D;

namespace Default.Namespace;

public class TeleportPortal : ParticleSystem
{
    private const int PARTS_COUNT = 5;

    protected Portal portal;

    public TeleportPortal(Portal _portal)
        : base(Mokus2DGame.LoadSpriteData("common/McTeleportPartBlack"), 5)
    {
        portal = _portal;
    }

    public override void Update(float time)
    {
        base.Update(time);
        for (int i = 0; i < 5; i++)
        {
            Particle particle = portal.Particles[i];
            Particle particle2 = Particles[i];
            particle2.Position = particle.Position;
            particle2.Scale = particle.Scale;
        }
    }
}
