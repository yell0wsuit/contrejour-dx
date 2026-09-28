using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class ButterFly : FlyBase
{
    private float horizontalStep;

    private readonly float step;

    public ButterFly(Particle _particle, float _scale)
        : base(_particle, _scale)
    {
        step = Maths.Random(0.01f, 0.03f);
    }

    public override void Update(float time)
    {
        horizontalStep += step;
        targetPosition = ChooseTarget();
        base.Update(time);
    }

    private Vector2 ChooseTarget()
    {
        return new Vector2(initialPosition.X + (5f * (Maths.Sin(horizontalStep) + 1f)), initialPosition.Y + (3.5f * Maths.Sin(verticalStep)));
    }
}
