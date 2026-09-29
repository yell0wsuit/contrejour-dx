using System;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay;

public class LastParticles : BlackFall
{
    public LastParticles()
        : base("chapter5/Mc5ChapterParticle")
    {
        SpeedMult = 5f;
    }

    protected override void InitParams()
    {
        base.InitParams();
        ParticlesScale = new RandomRange(0.9f, 0.6f);
    }

    public override void InitParticle(GravityParticle gravityParticle)
    {
        if (Maths.Random() < 0.1f)
        {
            gravityParticle.Scale = Maths.Random(3f, 3.5f);
        }
        base.InitParticle(gravityParticle);
        float num = ParticlesScale.Value + ParticlesScale.Offset;
        gravityParticle.OpacityFloat = Math.Max((num - gravityParticle.Scale) / num, 0.05f);
    }
}
