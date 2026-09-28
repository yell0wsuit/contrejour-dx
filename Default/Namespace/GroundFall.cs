using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class GroundFall : GravityParticleSystem
{
    private const float SCALE_STEP = 0.015f;

    protected bool black;

    public GroundFall(ContreJourGame game)
        : base(game.Choose("common/McGroundPart", "common/McGroundPartBlack", "chapter4/McGroundPartWhite", null, "McGroundPart_6"))
    {
        black = game.BlackSide;
        base.Angle = new RandomRange(270f, 0f);
        base.Speed = new RandomRange(40f, 20f);
        base.AngularSpeed = new RandomRange(0f, 0f);
        base.ParticlesScale = new RandomRange(1f, 0.3f);
        base.StartOpacity = new RandomRange(200f, 50f);
    }

    public override void UpdateParticleTime(Particle particle, float time)
    {
        base.UpdateParticleTime(particle, time);
        particle.Scale -= time / 2f;
        if (particle.Scale <= 0f)
        {
            particle.Visible = false;
        }
    }
}
