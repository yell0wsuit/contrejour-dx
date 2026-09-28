using System;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class FlyBase : IUpdatable
{
    private readonly CosOpacityChanger opacityChanger;

    protected Vector2 initialPosition;

    protected Particle particle;

    protected float stepY;

    protected Vector2 targetPosition;

    protected float verticalStep;

    protected float verticalStepDiff;

    public FlyBase(Particle _particle, float _scale)
    {
        particle = _particle;
        particle.Scale = _scale;
        initialPosition = _particle.Position;
        targetPosition = initialPosition;
        opacityChanger = new CosOpacityChanger(particle, 0f, Maths.Random(0.5f, 0.6f), Maths.Random(0.01f, 0.07f));
        InitParams();
    }

    public virtual void Update(float time)
    {
        Vector2 vector = new(Math.Min(Math.Abs((targetPosition.X - particle.Position.X) / (targetPosition.Y - particle.Position.Y) * stepY), 1f), stepY);
        particle.Position = VectorUtil.StepTo(particle.Position, targetPosition, vector.Length());
        verticalStep += verticalStepDiff;
        opacityChanger.Update(time);
    }

    private void InitParams()
    {
        stepY = Maths.Random(0.25f, 0.75f);
        verticalStep = Maths.Random((float)Math.PI * -2f, (float)Math.PI * 2f);
        verticalStepDiff = Maths.Random(0.01f, 0.02f);
    }
}
