using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class WhiteSmoke : GravityParticleSystem
{
    public bool ScaleDownOnDestroy = true;

    public float OpacityStep { get; set; }

    public float ScaleStep { get; set; }

    public float MaxOpacity { get; set; }

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
        MaxOpacity = 255f;
    }

    public override void InitParticle(GravityParticle gravityParticle)
    {
        base.InitParticle(gravityParticle);
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
            particle.OpacityByte = (int)Maths.StepTo(particle.OpacityByte, MaxOpacity, MaxOpacity / 4f);
            if (particle.OpacityByte >= MaxOpacity)
            {
                particle.Tag = null;
            }
        }
        else
        {
            float num = OpacityStep * time / 255f;
            if ((double)particle.OpacityFloat < 0.5 && ScaleDownOnDestroy)
            {
                num *= 4f;
            }
            particle.OpacityFloat -= num;
        }
        if ((double)particle.OpacityFloat < 0.5 && ScaleDownOnDestroy)
        {
            particle.Scale -= ScaleStep * time;
        }
        else
        {
            particle.Scale += ScaleStep * time;
        }
        if (particle.OpacityByte <= 0 || (ScaleStep < 0f && (particle.Scale < 2f || particle.OpacityByte > 150)))
        {
            particle.Visible = false;
            InitParticle((GravityParticle)particle);
        }
    }
}
