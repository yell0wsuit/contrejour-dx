using System;

using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Actions.Particles;

public abstract class GridLinesAction : GridAction
{
    protected float LinesDelay { get; set; }

    protected float ParticleEffectSeconds { get; set; }

    protected float OneLineDelay { get; set; }

    protected GridLinesAction Initialize(float linesDelay, float particleEffectSeconds)
    {
        _ = Initialize();
        this.LinesDelay = linesDelay;
        this.ParticleEffectSeconds = particleEffectSeconds;
        return this;
    }

    internal override void Start(float time)
    {
        CalculateLineDelay();
        base.Start(time);
    }

    protected void CalculateLineDelay()
    {
        OneLineDelay = LinesDelay / Grid.GridSize.Y;
    }

    protected override ITween CreateParticleUpdater(Node particle, int x, int y)
    {
        throw new NotImplementedException();
    }

    protected virtual float GetLineDelay(int y)
    {
        return y * OneLineDelay;
    }

    protected abstract ITween CreateDelayedParticleUpdater(Node particle, int x, int y);
}
