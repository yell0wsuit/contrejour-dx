
using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Actions.Particles;

public abstract class LinesFlyBase : GridLinesAction
{
    protected float ParticlesOffset { get; set; }

    protected virtual LinesFlyBase Initialize(float linesDelay, float particleEffectSeconds, float particlesOffset)
    {
        ParticlesOffset = particlesOffset;
        _ = Initialize(linesDelay, particleEffectSeconds);
        return this;
    }

    internal override void Start(float time)
    {
        Grid.ResetTransformations();
        CalculateLineDelay();
        if (Grid.Children.Count < Grid.GridSize.Length() * 2f)
        {
            Grid.CreateParticles();
        }
        for (int i = 0; i < Grid.Children.Count; i++)
        {
            Node particle = Grid.Children[i];
            Vector2 particlePosition = Grid.GetParticlePosition(i);
            CreateAction(particle, (int)particlePosition.X, (int)particlePosition.Y);
        }
    }

    protected override float GetLineDelay(int y)
    {
        float lineDelay = base.GetLineDelay(y);
        return lineDelay + Maths.Random(0f - OneLineDelay, OneLineDelay);
    }
}
