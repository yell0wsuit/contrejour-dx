using System;

using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class LastParticles : BlackFall
{
    public LastParticles()
        : base("chapter5/Mc5ChapterParticle")
    {
        SpeedMult = 5f;
    }

    protected override void initParams()
    {
        base.initParams();
        base.ParticlesScale = new RandomRange(0.9f, 0.6f);
    }

    public override void initParticle(GravityParticle gravityParticle)
    {
        if (Maths.Random() < 0.1f)
        {
            gravityParticle.Scale = Maths.Random(3f, 3.5f);
        }
        base.initParticle(gravityParticle);
        float num = base.ParticlesScale.Value + base.ParticlesScale.Offset;
        gravityParticle.OpacityFloat = Math.Max((num - gravityParticle.Scale) / num, 0.05f);
    }
}
