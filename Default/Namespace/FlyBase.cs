using System;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class FlyBase : IUpdatable
{
    private readonly CosOpacityChanger opacityChanger;

    protected Vector2 InitialPosition { get; set; }

    protected Particle Particle { get; set; }

    protected float StepY { get; set; }

    protected Vector2 targetPosition;

    protected float VerticalStep { get; set; }

    private float verticalStepDiff;

    public FlyBase(Particle particle, float scale)
    {
        this.Particle = particle;
        this.Particle.Scale = scale;
        InitialPosition = particle.Position;
        targetPosition = InitialPosition;
        opacityChanger = new CosOpacityChanger(this.Particle, 0f, Maths.Random(0.5f, 0.6f), Maths.Random(0.01f, 0.07f));
        InitParams();
    }

    public virtual void Update(float time)
    {
        Vector2 vector = new(Math.Min(Math.Abs((targetPosition.X - Particle.Position.X) / (targetPosition.Y - Particle.Position.Y) * StepY), 1f), StepY);
        Particle.Position = VectorUtil.StepTo(Particle.Position, targetPosition, vector.Length());
        VerticalStep += verticalStepDiff;
        opacityChanger.Update(time);
    }

    private void InitParams()
    {
        StepY = Maths.Random(0.25f, 0.75f);
        VerticalStep = Maths.Random((float)Math.PI * -2f, (float)Math.PI * 2f);
        verticalStepDiff = Maths.Random(0.01f, 0.02f);
    }
}
