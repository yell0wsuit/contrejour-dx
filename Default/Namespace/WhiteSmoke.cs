using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class WhiteSmoke : GravityParticleSystem
{
    public bool ScaleDownOnDestroy = true;

    protected float maxOpacity;

    protected float opacityStep;

    protected float scaleStep;

    public float OpacityStep
    {
        get => opacityStep;
        set => opacityStep = value;
    }

    public float ScaleStep
    {
        get => scaleStep;
        set => scaleStep = value;
    }

    public float MaxOpacity
    {
        get => maxOpacity;
        set => maxOpacity = value;
    }

    public virtual Vector2 SmokePosition
    {
        get => new(horizontalPosition.Value, verticalPosition.Value);
        set
        {
            horizontalPosition.Value = value.X;
            verticalPosition.Value = value.Y;
        }
    }

    public WhiteSmoke(string textureName)
        : this(textureName, 0)
    {
    }

    public WhiteSmoke(string textureName, int maxParticles)
        : base(Mokus2DGame.LoadSpriteData(textureName), maxParticles)
    {
        Speed = new RandomRange(250f, 80f);
        ParticlesScale = new RandomRange(2f, 1f);
        maxOpacity = 255f;
    }

    public override void initParticle(GravityParticle gravityParticle)
    {
        base.initParticle(gravityParticle);
        gravityParticle.Tag = this;
        gravityParticle.OpacityByte = 1;
    }

    public override void UpdateParticleTime(Particle particle, float time)
    {
        if (!particle.Visible)
        {
            return;
        }
        base.UpdateParticleTime(particle, time);
        if (particle.Tag != null)
        {
            particle.OpacityByte = (int)Maths.StepTo(particle.OpacityByte, maxOpacity, maxOpacity / 4f);
            if (particle.OpacityByte >= maxOpacity)
            {
                particle.Tag = null;
            }
        }
        else
        {
            float num = opacityStep * time / 255f;
            if ((double)particle.OpacityFloat < 0.5 && ScaleDownOnDestroy)
            {
                num *= 4f;
            }
            particle.OpacityFloat -= num;
        }
        if ((double)particle.OpacityFloat < 0.5 && ScaleDownOnDestroy)
        {
            particle.Scale -= scaleStep * time;
        }
        else
        {
            particle.Scale += scaleStep * time;
        }
        if (particle.OpacityByte <= 0 || (scaleStep < 0f && (particle.Scale < 2f || particle.OpacityByte > 150)))
        {
            particle.Visible = false;
            initParticle((GravityParticle)particle);
        }
    }
}
