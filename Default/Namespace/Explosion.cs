using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class Explosion : GravityParticleSystem
{
    protected float opacityStep;

    protected float scaleStep;

    public float ScaleStep
    {
        get
        {
            return scaleStep;
        }
        set
        {
            scaleStep = value;
        }
    }

    public float OpacityStep
    {
        get
        {
            return opacityStep;
        }
        set
        {
            opacityStep = value;
        }
    }

    public Explosion(string textureName)
        : base(textureName)
    {
        opacityStep = 300f;
        scaleStep = 6f;
        base.StartOpacity = new RandomRange(220f, 35f);
        base.Speed = new RandomRange(140f, 20f);
        base.ParticlesScale = new RandomRange(2f, 1f);
        base.Angle = new RandomRange(0f, 3600f);
    }

    public override void initParticle(GravityParticle gravityParticle)
    {
        base.initParticle(gravityParticle);
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
            particle.OpacityByte -= (int)(opacityStep * time);
            particle.Scale += scaleStep * time;
            if (particle.OpacityByte <= 0 || particle.Scale < 0f)
            {
                particle.Visible = false;
            }
        }
    }
}
