using System;

using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Actions.Particles;

public abstract class GridLinesAction : GridAction
{
    protected float linesDelay;

    protected float particleEffectSeconds;

    protected float oneLineDelay;

    protected GridLinesAction Initialize(float linesDelay, float particleEffectSeconds)
    {
        _ = Initialize();
        this.linesDelay = linesDelay;
        this.particleEffectSeconds = particleEffectSeconds;
        return this;
    }

    internal override void Start(float time)
    {
        CalculateLineDelay();
        base.Start(time);
    }

    protected void CalculateLineDelay()
    {
        oneLineDelay = linesDelay / Grid.GridSize.Y;
    }

    protected override ITween CreateParticleUpdater(Node particle, int x, int y)
    {
        throw new NotImplementedException();
    }

    protected virtual float GetLineDelay(int y)
    {
        return y * oneLineDelay;
    }

    protected abstract ITween CreateDelayedParticleUpdater(Node particle, int x, int y);
}
