using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay;

public class WhiteSnow : SnowFall
{
    public WhiteSnow()
        : base("common/McSnowParticle")
    {
    }

    protected override void InitParams()
    {
        base.InitParams();
        ParticlesScale = new RandomRange(1f, 0.4f);
        StartOpacity = new RandomRange(255f, 0f);
    }
}
