using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class SnowFall : BlackFall
{
    public SnowFall()
        : base("common/McSnowParticle")
    {
        SpeedMult = 2.5f;
    }

    public SnowFall(string textureName)
        : base(textureName)
    {
    }

    protected override void initParams()
    {
        base.initParams();
        AngularSpeed = new RandomRange(0f, 0f);
        ParticlesScale = new RandomRange(1.75f, 0.75f);
    }
}
