using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class WhiteSnow : SnowFall
{
    public WhiteSnow()
        : base("common/McSnowParticle")
    {
    }

    protected override void initParams()
    {
        base.initParams();
        base.ParticlesScale = new RandomRange(1f, 0.4f);
        base.StartOpacity = new RandomRange(255f, 0f);
    }
}
