using Default.Namespace;
using Microsoft.Xna.Framework;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Actions.Particles;

public abstract class LinesFlyBase : GridLinesAction
{
	protected float particlesOffset;

	protected virtual LinesFlyBase Initialize(float linesDelay, float particleEffectSeconds, float particlesOffset)
	{
		this.particlesOffset = particlesOffset;
		Initialize(linesDelay, particleEffectSeconds);
		return this;
	}

	internal override void Start(float time)
	{
		base.Grid.ResetTransformations();
		CalculateLineDelay();
		if ((float)base.Grid.Children.Count < base.Grid.GridSize.Length() * 2f)
		{
			base.Grid.CreateParticles();
		}
		for (int i = 0; i < base.Grid.Children.Count; i++)
		{
			Node particle = base.Grid.Children[i];
			Vector2 particlePosition = base.Grid.GetParticlePosition(i);
			CreateAction(time, particle, (int)particlePosition.X, (int)particlePosition.Y);
		}
	}

	protected override float GetLineDelay(int y)
	{
		float lineDelay = base.GetLineDelay(y);
		return lineDelay + Maths.Random(0f - oneLineDelay, oneLineDelay);
	}
}
