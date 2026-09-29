using System;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Actions.Particles
{
    public class LinesFlyOut : LinesFlyBase
    {
        private static readonly Pool<LinesFlyOut> pool = new(() => new LinesFlyOut());

        public static LinesFlyOut New(float linesDelay, float particleEffectSeconds, float particlesOffset)
        {
            return pool.New().Initialize(linesDelay, particleEffectSeconds, particlesOffset);
        }

        protected LinesFlyOut()
        {
        }

        protected new LinesFlyOut Initialize(float linesDelay, float particleEffectSeconds, float particlesOffset)
        {
            _ = base.Initialize(linesDelay, particleEffectSeconds, particlesOffset);
            return this;
        }

        protected override ITween CreateDelayedParticleUpdater(Node particle, int x, int y)
        {
            throw new NotImplementedException();
        }

        public new void Free()
        {
            pool.Free(this);
        }
    }
}
