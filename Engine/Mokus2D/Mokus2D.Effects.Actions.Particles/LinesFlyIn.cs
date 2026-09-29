using System;

using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Actions.Particles;

public class LinesFlyIn : LinesFlyBase
{
    private static readonly Pool<LinesFlyIn> pool = new(() => new LinesFlyIn());

    public static LinesFlyIn New(float linesDelay, float particleEffectSeconds, float particlesOffset)
    {
        return pool.New().Initialize(linesDelay, particleEffectSeconds, particlesOffset);
    }

    protected LinesFlyIn()
    {
    }

    protected new LinesFlyIn Initialize(float linesDelay, float particleEffectSeconds, float particlesOffset)
    {
        _ = base.Initialize(linesDelay, particleEffectSeconds, particlesOffset);
        return this;
    }

    protected override float GetLineDelay(int y)
    {
        return LinesDelay - base.GetLineDelay(y);
    }

    protected override ITween CreateDelayedParticleUpdater(Node particle, int x, int y)
    {
        _ = particle.Position;
        int num = (Maths.Random(2) * 2) - 1;
        float num2 = ParticlesOffset * Maths.Random(0.7f, 1.3f);
        particle.Position += new Vector2(0f, 0f - num2);
        particle.Scale = Maths.Random(0.2f, 0.5f);
        particle.OpacityFloat = 0f;
        particle.RotationRadians = (float)(num * Math.PI);
        _ = ParticleEffectSeconds;
        throw new NotImplementedException();
    }

    public new void Free()
    {
        pool.Free(this);
    }
}
