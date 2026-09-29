using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay;

public class Explosion : GravityParticleSystem
{
    public float ScaleStep { get; set; }

    public float OpacityStep { get; set; }

    public Explosion(string textureName)
        : base(textureName)
    {
        OpacityStep = 300f;
        ScaleStep = 6f;
        StartOpacity = new RandomRange(220f, 35f);
        Speed = new RandomRange(140f, 20f);
        ParticlesScale = new RandomRange(2f, 1f);
        Angle = new RandomRange(0f, 3600f);
    }

    public override void InitParticle(GravityParticle gravityParticle)
    {
        base.InitParticle(gravityParticle);
        if (gravityParticle.Position != Vector2.Zero)
        {
            gravityParticle.Speed = VectorUtil.ToVector(gravityParticle.Speed.Length(), Maths.Atan2(gravityParticle.Position.Y, gravityParticle.Position.X));
        }
    }

    public override void UpdateParticleTime(Particle particle, float time)
    {
        if (particle.Visible)
        {
            base.UpdateParticleTime(particle, time);
            particle.OpacityByte -= (int)(OpacityStep * time);
            particle.Scale += ScaleStep * time;
            if (particle.OpacityByte <= 0 || particle.Scale < 0f)
            {
                particle.Visible = false;
            }
        }
    }
}
